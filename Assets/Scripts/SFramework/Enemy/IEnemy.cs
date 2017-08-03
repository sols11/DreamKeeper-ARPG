using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

namespace SFramework
{
    /// <summary>
    /// + Enemy的所有属性
    /// + EnemyMeditor
    /// + Enemy事件
    /// Enemy的数据层
    /// Enemy的攻击属性由WeaponData决定 ，防御属性由ClothData决定，其他属性由Initialize设定的基础值决定
    /// 一个Enemy需要的组件：rgbd,collider,enemyWeapon,animator,bt,iEnemyMono,navMeshAgent,audiosource
    /// </summary>
    public class IEnemy : ICharacter
	{
        public BehaviorTree bt { get; set; }
        public NavMeshAgent navMeshAgent { get; set; }

        public EnemyType Type { get; set; }
        protected EnemyMediator EnemyMedi { get; set; }

        public IEnemy(GameObject gameObject):base(gameObject)
		{
            Type = EnemyType.Monster;
			if (GameObjectInScene != null)
			{
				animator = GameObjectInScene.GetComponent<Animator>();
				Rgbd = GameObjectInScene.GetComponent<Rigidbody>();
                bt = GameObjectInScene.GetComponent<BehaviorTree>();
                navMeshAgent = GameObjectInScene.GetComponent<NavMeshAgent>();
                // 关联中介者
                EnemyMedi = new EnemyMediator(this);
                EnemyMedi.Initialize();
            }
		}

        public override void Release()
        {
            EnemyMedi.EnemyMono.Release();    // 先释放再销毁
            if (GameObjectInScene != null)
                GameObject.Destroy(GameObjectInScene);
        }
        public virtual EnemyAction Hurt(EnemyHurtAttr enemyHurtAttr)
		{
            return EnemyAction.Hurt;
		}

        public virtual void WhenPlayerDead()
        {
            bt.DisableBehavior();
        }
	}
}
