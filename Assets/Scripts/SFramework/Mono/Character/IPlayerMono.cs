using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    ///+ iPlayerWeapon 预制属性，进行初始化
    ///+ PlayerMediator
    ///+ 动画事件
    ///由PlayerMediator初始化，不使用Mono生命周期
    ///Player的MonoBehaviour脚本，用于武器碰撞检测和动画事件
    /// </summary>
    public abstract class IPlayerMono : ICharacterMono
	{
		public IPlayerWeapon iPlayerWeapon;      //在预制时赋好的变量
        protected AnimatorStateInfo stateInfo;
        public PlayerMediator PlayerMedi { get; set; } 

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initialize只在第一次创建时由IPlayer的构造函数执行初始化代码，
        /// 之后切换Scene时都不用再次初始化
        /// </summary>
        public override void Initialize() {
            base.Initialize();
        }
        /// <summary>
        /// Release也不会删除GO。
        /// </summary>
        public override void Release()
        {
            iPlayerWeapon.Release();
        }


        /// <summary>
        /// 提供给EnemyAttack调用
        /// </summary>
        /// <param name="damage"></param>
        public virtual void Hurt(PlayerHurtAttr _playerHurtAttr)
		{
            PlayerMedi.Player.Hurt(_playerHurtAttr);
		}
        /// <summary>
        /// 用于动画调用使其移动
        /// </summary>
        /// <param name="v"></param>
        public virtual void velocityForward(float v)
        {
            Rgbd.velocity += transform.forward * v;
        }
        public virtual void TrailSwitch(int open) { }
    }
}