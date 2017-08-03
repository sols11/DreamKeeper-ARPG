using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SFramework
{
    /// <summary>
    /// + Player的所有属性
    /// + PlayerMeditor
    /// + 数据修改
    /// + 数据存取
    /// Player的数据层
    /// Player的属性由装备和基础值决定
    /// </summary>
	public class IPlayer : ICharacter
	{
        protected int m_MedicineNum;

		protected bool m_CanMove = false;   //CanMovePosition
		protected float h;
		protected float v;
        protected GameData gameData;

        // public float AvoidSpeed { get; set; } 用MoveSpeed*2代替
		public float Speed { get; set; }

        //Player的HP,SP需要更新UI
        public override int CurrentHP
        {
            get { return base.CurrentHP; }
            set { base.CurrentHP = value;
                UpdateHP_SP();
            }
        }
        public override int CurrentSP
        {
            get { return base.CurrentSP; }
            set
            {
                base.CurrentSP = value;
                UpdateHP_SP();
            }
        }
        //道具
        public int Gold { get; set; }
        // 为了方便使用和判断单独写出MedicineNum
        public int MedicineNum {
            get { return m_MedicineNum; }
            set {
                m_MedicineNum = value;
                UpdateMedicineNum();
            }
        }
        // PLayer存放装备数据和背包数据
        // 数组索引与UIInventory及背包中的显示顺序是对应的
        public int[] Fit { get; set; }
        public IEquip[] EquipPack { get; set; }
        public IProp[] PropPack { get; set; }
        //隐藏属性
        public int MonsterResist { get; set; }
        public int WarriorResist { get; set; }
        public int MagianResist { get; set; }
        /// <summary>
        /// 在设置时将相关值赋为0，主要是设置false时需要
        /// </summary>
        public bool CanMove
		{
			get { return m_CanMove; }
			set { h = 0; v = 0; m_CanMove = value; }
		}
		public bool CanRotate { get; set; }
        public PlayerMediator PlayerMedi { get; set; }
        
        /// <summary>
        /// Initialize只在第一次创建时执行初始化代码，之后切换Scene时都不用再次初始化，所以Data也没有改变
        /// </summary>
        /// <param name="gameObject"></param>
        public IPlayer(GameObject gameObject):base(gameObject)
        {
            EquipPack = new IEquip[35];
            PropPack = new IProp[35];
            Fit = new int[8]{ 99,99,99,99,99,99,99,99}; // 初始值>34表示未装备
            if (GameObjectInScene != null)
            {
                animator = GameObjectInScene.GetComponent<Animator>();
                Rgbd = GameObjectInScene.GetComponent<Rigidbody>();
                // 关联中介者
                PlayerMedi = new PlayerMediator(this);
                PlayerMedi.Initialize();
            }
            // LoadData(); 请在子类构造函数最尾处调用
        }

        public override void Release()
        {
            PlayerMedi.PlayerMono.Release();
        }
        public virtual void Hurt(PlayerHurtAttr _playerHurtAttr) { }
        public virtual void Roared() { }

        public override void Dead()
        {
            base.Dead();
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.PlayerDead);    // 触发死亡事件
        }

        public void LoadData()
        {
            // Load
            gameData = GameMainProgram.Instance.gameDataMgr.Load();
            Name = gameData.Name;
            Rank = gameData.Rank;
            Gold = gameData.Gold;
            // 背包
            for (int i = 0; i < EquipPack.Length; i++)
                EquipPack[i] = gameData.EquipPack[i];
            for (int i = 0; i < PropPack.Length; i++)
                PropPack[i] = gameData.PropPack[i];
            for (int i = 0; i < Fit.Length; i++)
            {
                // 每更新一个值装备一个。
                if (i == (int)FitType.Medicine)
                    PlayerMedi.ChangeProp(gameData.Fit[i]);
                else
                    PlayerMedi.ChangeEquip(gameData.Fit[i]);
            }
        }
        public void SaveData()
        {
            gameData.Name = Name;
            gameData.Rank = Rank;
            gameData.Gold = Gold;
            // 背包
            for (int i = 0; i < EquipPack.Length; i++)
                gameData.EquipPack[i] = EquipPack[i];
            for (int i = 0; i < PropPack.Length; i++)
                gameData.PropPack[i] = PropPack[i];
            for (int i = 0; i < Fit.Length; i++)
                gameData.Fit[i] = Fit[i];
            // Save
            GameMainProgram.Instance.gameDataMgr.Save();
        }

        /// <summary>
        /// 添加背包中的装备
        /// </summary>
        public bool AddItem(IEquip _Equip)
        {
            int i = 0;
            while (EquipPack[i] != null)    // 找到一个空位
                i++;
            if (i >= EquipPack.Length)
                return false;
            EquipPack[i] = _Equip;
            return true;
        }
        /// <summary>
        /// 添加背包中的道具，如果有相同的那么添加数目
        /// </summary>
        public bool AddItem(IProp _Prop)
        {
            int i = 0;
            int emptyIndex = 99;    // 找到的第一个空位
            for(;i<PropPack.Length;i++)
            {
                if(PropPack[i]==null)
                {
                    if(emptyIndex==99)
                        emptyIndex = i;
                    continue;
                }
                // 找到相同的道具
                if(PropPack[i].Name == _Prop.Name)
                {
                    if (PropPack[i].Num < PropPack[i].MaxNum)
                    {
                        PropPack[i].Num++;
                        return true;
                    }
                    return false;
                }
            }
            // 背包已满，未找到空位
            if (emptyIndex == 99)
            {
                Debug.Log("背包物品达到上限，不可添加");
                return false;
            }
            // 添加到空位
            PropPack[emptyIndex] = _Prop;
            return true;
        }

        /// <summary>
        /// 移除装备，用于普通逻辑
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></returns>
        public bool RemoveEquip(int _ID)
        {
            if (_ID >= EquipPack.Length)
            {
                Debug.Log("数组下标越界");
                return false;
            }
            else if (EquipPack[_ID] == null)
            {
                Debug.Log("EquipPack的"+ _ID+"位置为null，无法移除");
                return false;
            }
            else
            {
                EquipPack[_ID] = null;
                return true;
            }
        }
        /// <summary>
        /// 移除道具，如果数目为0
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></returns>
        public bool RemoveProp(int _ID)
        {
            if (_ID >= PropPack.Length)
            {
                Debug.Log("数组下标越界");
                return false;
            }
            else if (PropPack[_ID] == null)
            {
                Debug.Log("PropPack的" + _ID + "位置为null，无法移除");
                return false;
            }
            else
            {
                if(PropPack[_ID].Num<=0)
                    PropPack[_ID] = null;
                return true;
            }
        }

        private void UpdateHP_SP()
        {
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.PlayerHP_SP);
        }
        private void UpdateMedicineNum()
        {
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.MedicineNum);
        }


    }
}