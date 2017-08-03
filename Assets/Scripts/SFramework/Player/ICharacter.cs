using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace SFramework
{
	/// <summary>
	/// 游戏中任何活的角色
    /// + 通用属性，包含8大基本属性
    /// - 死亡判断
    /// 会细分为Player和Enemy两种，所以只作为通用接口，为以后可能的修改做准备
    /// </summary>
    public abstract class ICharacter
	{
		protected int m_MaxHP=10;   // 保持一个底线值是为了避免一开始HP就为0
		protected int m_CurrentHP=10;
        protected int m_MaxSP = 100;
        protected int m_CurrentSP = 100;
        protected bool m_IsDead = false;			// 是否已挂
		protected bool m_HasCheckDead = false;      // 是否确认过阵亡事件
		//动画状态机
		public Animator animator;
		protected AnimatorStateInfo stateInfo;

		//属性，供外部获取
		public string Name { get; protected set; }
		public float MoveSpeed { get; set; }
		public float RotSpeed { get; set; }
        public int Rank { get; set; }
        /// <summary>
        /// set时将CurrentHP回复满
        /// </summary>
        public int MaxHP { get { return m_MaxHP; } set { m_MaxHP = value < 0 ? 0 : value;
                CurrentHP = MaxHP;
            } }
		public virtual int CurrentHP
        {
            get { return m_CurrentHP; }
            set
            {
                m_CurrentHP = value >= MaxHP ? MaxHP : value;
                if (m_CurrentHP <= 0)
                {
                    m_CurrentHP = 0;
                    Dead();
                }
                HPpercent = m_CurrentHP * 1.0f / MaxHP;
            }
        }
        public float HPpercent { get; protected set; }
        public int MaxSP { get { return m_MaxSP; } set { m_MaxSP = value < 0 ? 0 : value;
                CurrentSP = MaxSP;
            } }
        public virtual int CurrentSP
        {
            get { return m_CurrentSP; }
            set
            {
                m_CurrentSP = value >= MaxSP ? MaxSP : value;
                m_CurrentSP = value < 0 ? 0 : m_CurrentSP;
                SPpercent = m_CurrentSP * 1.0f / MaxSP;
            }
        }
        public float SPpercent { get; protected set; }
        //装备属性
        public int AttackPoint { get; set; }
        public int DefendPoint { get; set; }
        public int CritPoint { get; set; }
        public SpecialAbility Special { get; set; }
        //设置对应物体
        public GameObject GameObjectInScene { get; set; }
		public bool IsDead { get { return m_IsDead; } }
        public Rigidbody Rgbd { get; set; }

        //构造函数
        public ICharacter(GameObject gameObject)
        {
            GameObjectInScene = gameObject;
        }

        public virtual void Initialize() { }
		public virtual void Release() { }
		public virtual void Update() { }
		public virtual void FixedUpdate() { }

        #region 陣亡
        // 陣亡
        public virtual void Dead()
		{
			if (m_IsDead == true)
				return;
			m_IsDead = true;
			m_HasCheckDead = false;
		}


		// 是否確認陣亡過
		public bool CheckDeadEvent()
		{
			if (m_HasCheckDead)
				return true;
			m_HasCheckDead = true;
			return false;
		}

		#endregion
	}
}
