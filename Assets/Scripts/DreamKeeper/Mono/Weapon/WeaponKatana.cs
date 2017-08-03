 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
	/// <summary>
	/// 武器类，负责伤害计算，碰撞检测和传参,特效生成
	/// 武器一般不添加刚体，而角色必须有刚体
	/// </summary>
	public class WeaponKatana : IPlayerWeapon
	{
        private KatanaAnimEvent katanaAnimEvent; // 对应的PlayerMono
        private string defendEffectPath;
        private string parriedEffectPath;

		public override void Initialize()
		{
            base.Initialize();
            hitEffectPath = @"Particles\Boom_Splash";   // Player的攻击频率过快，且敌人可能有多个，所以缓冲池会产生多个
            defendEffectPath = @"Particles\Boom_White";
            parriedEffectPath = @"Particles\Boom_Orange";
            katanaAnimEvent = PlayerMedi.PlayerMono as KatanaAnimEvent;
        }

        private void InstantiateEffect()
        {
            if (EnemyReturn == EnemyAction.Defend)
            {
                if(!katanaAnimEvent.IsHeavyAttack()) // 防住
                    resourcesMgr.LoadAsset(defendEffectPath, true,transform.position, Quaternion.identity);
            }
            else if(EnemyReturn == EnemyAction.Parry)
            {
                if (katanaAnimEvent.IsHeavyAttack()) //被挡开
                {
                    resourcesMgr.LoadAsset(parriedEffectPath, true,transform.position, Quaternion.identity);
                    katanaAnimEvent.Parried();
                }
                else // 防住
                    resourcesMgr.LoadAsset(defendEffectPath, true,transform.position, Quaternion.identity);
            }
            else
                resourcesMgr.LoadAsset(hitEffectPath, true,transform.position, Quaternion.Euler(90, 0, 0));
        }

        protected override void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.layer==(int)ObjectLayer.Enemy)
			{
				if (!enemyList.Contains(col.gameObject))//攻击存在性判断
				{
					enemyList.Add(col.gameObject);
					RealAttack = BasicAttack * AttackFactor;
                    //传参给enemy告知受伤
                    TransformForward = PlayerMedi.PlayerMono.transform.forward;
					EnemyHurtAttribute.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward);
                    EnemyReturn = col.GetComponent<IEnemyMono>().Hurt(EnemyHurtAttribute);
                    //特效
                    InstantiateEffect();
                }
            }
		}  // end_function
	}
}