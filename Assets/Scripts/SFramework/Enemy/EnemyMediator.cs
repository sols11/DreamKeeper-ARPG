using System;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// IEnemy,IEnemyMono,IEnemyWeapon3个关联类的中介者
    /// 和PlayerMediator不同的是，Enemy没有Fit[]，所以需要专门存储武器和防具
    /// 负责更换装备
    /// </summary>
    public class EnemyMediator
    {
        public IEnemy Enemy { get; set; }
        public IEnemyMono EnemyMono { get; set; }
        public IEnemyWeapon EnemyWeapon { get; set; }
        public IEquip WeaponData { get; set; }  // 和PlayerMediator不同的是，Enemy没有Fit[]，所以需要专门存储武器和防具
        public IEquip ClothData { get; set; }

        public EnemyMediator(IEnemy _enemy)
        {
            Enemy = _enemy;
        }

        public void Initialize()
        {
            // 默认装备，可以在后续修改其对象
            EnemyMono = Enemy.GameObjectInScene.GetComponent<IEnemyMono>();
            if (EnemyMono != null)
            {
                EnemyMono.EnemyMedi = this;
                EnemyMono.AnimatorComponent = Enemy.animator;
                EnemyMono.Rgbd = Enemy.Rgbd;
                EnemyMono.BdTree = Enemy.bt;
                EnemyMono.NavMeshAgentComponent = Enemy.navMeshAgent;
                EnemyWeapon = EnemyMono.iEnemyWeapon;
                EnemyMono.Initialize();
                if (EnemyWeapon != null)
                {
                    EnemyWeapon.EnemyMedi = this;
                    EnemyWeapon.Initialize();
                    EnemyMono.WeaponCollider = EnemyWeapon.WeaponCollider;
                }
                else
                    Debug.LogError("iEnemyWeapon未赋值");
            }

            UpdateEnemyWeapon(EnemyMono.iEnemyWeapon);
        }
        /// <summary>
        /// 改变WeaponData,更新Player属性，更新WeaponMono属性
        /// </summary>
        /// <param name="_equipData"></param>
        public void ChangeEquip(IEquip _equipData)
        {
            if (_equipData == null)
            {
                Debug.Log("_equipData为空！");
                return;
            }
            // 切换不同的装备
            switch (_equipData.Type)
            {
                case FitType.Weapon:
                    ChangeEquip_aux(WeaponData, _equipData);
                    break;
                case FitType.Cloth:
                    ChangeEquip_aux(ClothData, _equipData);
                    break;
            }
        }
        private void ChangeEquip_aux(IEquip _dropEquip, IEquip _dressOnEquip)
        {
            // 无论是否有装备可以卸下都可以穿上新装备
            if (_dropEquip != null)
            {
                Enemy.MoveSpeed -= _dropEquip.Speed;
                Enemy.MaxHP -= _dropEquip.HP;
                Enemy.MaxSP -= _dropEquip.SP;
                Enemy.AttackPoint -= _dropEquip.Attack;
                Enemy.DefendPoint -= _dropEquip.Defend;
                Enemy.CritPoint -= _dropEquip.Crit;
                if (_dropEquip.Type == FitType.Weapon)
                    Enemy.Special = SpecialAbility.无;
            }
            Enemy.MoveSpeed += _dressOnEquip.Speed;
            Enemy.MaxHP += _dressOnEquip.HP;
            Enemy.MaxSP += _dressOnEquip.SP;
            Enemy.AttackPoint += _dressOnEquip.Attack;
            Enemy.DefendPoint += _dressOnEquip.Defend;
            Enemy.CritPoint += _dressOnEquip.Crit;
            if (_dressOnEquip.Type == FitType.Weapon)
                Enemy.Special = _dressOnEquip.Special;
            // 更新完Player的属性后更新WeaponMono的属性
                UpdateEnemyWeapon(EnemyWeapon);

        }
        /// <summary>
        /// 设置WeaponData，使用的是装备的武器
        /// 就目前的实现来说，所有Weapon对象均共用同一个IWeaponMono，所以初始化时关联一个默认Weapon，切换武器时切换Weapon对象即可
        /// </summary>
        /// <param name="_weapon">哪一个PlayerWeapon</param>
        public void UpdateEnemyWeapon(IEnemyWeapon _EnemyWeapon)
        {
                _EnemyWeapon.BasicAttack = Enemy.AttackPoint;
                _EnemyWeapon.Crit = Enemy.CritPoint;
                _EnemyWeapon.Special = Enemy.Special;
        }
    }


}

