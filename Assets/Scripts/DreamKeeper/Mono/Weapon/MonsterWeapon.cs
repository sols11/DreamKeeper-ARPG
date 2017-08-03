using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class MonsterWeapon : IEnemyWeapon
    {
        private string hitEffectPath; // 打击特效

        public override void Initialize()
        {
            base.Initialize();
            hitEffectPath = @"Particles\EnemyEffect\Recoil_Metal";
        }

        protected override void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == (int)ObjectLayer.Player)
            {
                RealAttack = BasicAttack * AttackFactor;
                //传参给Player告知受伤
                TransformForward = EnemyMedi.EnemyMono.transform.forward;
                PlayerHurtAttr.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward, canDefeatedFly);
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
                resourcesMgr.LoadAsset(hitEffectPath, true, transform.position, Quaternion.identity);
            }
        }  // end_function
    }
}