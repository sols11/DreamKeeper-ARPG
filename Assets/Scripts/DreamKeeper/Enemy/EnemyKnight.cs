using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class EnemyKnight : IEnemy
    {
        //动画参数
        private string aniDefend = "Defend";
        private string aniHurt = "Hurt";
        private string aniDead = "Dead";

        public EnemyKnight(GameObject gameObject):base(gameObject)
		{
            Type = EnemyType.Warrior;
            MaxHP = 500;
            CurrentHP = MaxHP;
            MoveSpeed = 2;
            RotSpeed = 1;
            Name = "RoyalKnight";
            /// 行为树的开启由动画调用
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Update()
        {
			stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 需要使用
        }

        public override void FixedUpdate() { }

        public override EnemyAction Hurt(EnemyHurtAttr enemyHurtAttr)
        {
            if (!IsDead)
            {
                GameObjectInScene.transform.forward = -enemyHurtAttr.TransformForward;
                Rgbd.velocity = enemyHurtAttr.TransformForward * enemyHurtAttr.VelocityForward;
                // 防御
                if (stateInfo.IsName("Idle") || stateInfo.IsName("Walk")|| stateInfo.IsName("Defend"))
                {
                    animator.SetTrigger(aniDefend);
                    return EnemyAction.Parry;
                }
                animator.SetTrigger(aniHurt);
                CurrentHP -= enemyHurtAttr.Attack;
            }
            return EnemyAction.Hurt;
        }

        public override void Dead()
        {
            base.Dead();
            animator.SetTrigger(aniDead);
            bt.DisableBehavior();
        }
    }
}