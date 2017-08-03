using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
	/// <summary>
	/// 用于固定范围的Enemy攻击，负责战斗和碰撞检测，攻击相关属性
    /// 适用于带追踪功能的Enemy
	/// Trigger的开关交由Animation处理
	/// </summary>
	public class EnemyWeapon : IEnemyWeapon
	{
        public override void Initialize()
        {
            base.Initialize();
            IsOnlyPlayer = true;
        }

        protected override void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.layer == (int)ObjectLayer.Player)
			{
                RealAttack = BasicAttack * AttackFactor;
                //传参给Player告知受伤
                TransformForward = EnemyMedi.EnemyMono.transform.forward;
                PlayerHurtAttr.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward,canDefeatedFly);
                if (IsOnlyPlayer)
                {
                    if (OnlyPlayerMono == null)
                        OnlyPlayerMono = col.GetComponent<IPlayerMono>();
                    OnlyPlayerMono.Hurt(PlayerHurtAttr);
                }
                else
                {
                    col.GetComponent<IPlayerMono>().Hurt(PlayerHurtAttr);
                }
            }
        }  // end_function
	}
}