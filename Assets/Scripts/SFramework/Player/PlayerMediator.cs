using System;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// IPlayer,IPlayerMono,IPlayerWeapon3个关联类的中介者
    /// 负责更换装备
    /// </summary>
    public class PlayerMediator
    {
        public IPlayer Player { get; set; }
        public IPlayerMono PlayerMono { get; set; }
        public IPlayerWeapon PlayerWeapon { get; set; }

        public PlayerMediator(IPlayer _player)
        {
            Player = _player;
        }

        public void Initialize()
        {
            PlayerMono = Player.GameObjectInScene.GetComponent<IPlayerMono>();
            if (PlayerMono)
            {
                PlayerMono.PlayerMedi= this;
                PlayerMono.Rgbd = Player.Rgbd;
                PlayerMono.AnimatorComponent = Player.animator;
                PlayerWeapon = PlayerMono.iPlayerWeapon;
                PlayerMono.Initialize();
                if (PlayerWeapon != null)
                {
                    PlayerWeapon.PlayerMedi = this; // 引用
                    PlayerWeapon.Initialize();
                    PlayerMono.WeaponCollider = PlayerWeapon.WeaponCollider;
                }
                else
                    Debug.LogError("iPlayerWeapon未赋值");
                UpdatePlayerWeapon(PlayerMono.iPlayerWeapon);
            }
        }

        /// <summary>
        /// Player可以装备更多装备
        /// 更新Player属性，更新WeaponMono属性
        /// </summary>
        /// <param name="_equipData"></param>
        public void ChangeEquip(int _ID)
        {
            // 若ID超出或对应装备为空
            if (_ID>= Player.EquipPack.Length || Player.EquipPack[_ID] == null)
                return;

            // 对应装备
            IEquip _equip = Player.EquipPack[_ID];
            // 原本有装备的话卸下
            if (Player.Fit[(int)_equip.Type] < Player.EquipPack.Length)
                DropEquip(Player.EquipPack[Player.Fit[(int)_equip.Type]]);
            DressOnEquip(_equip);
            Player.Fit[(int)_equip.Type]=_ID;   // 更新Fit装备
            // 更新完Player的属性后更新WeaponMono的属性
            UpdatePlayerWeapon(PlayerWeapon);
        }

        public void DropEquip(IEquip _dropEquip)
        {
            if (_dropEquip != null)
            {
                Player.MoveSpeed -= _dropEquip.Speed / 10;
                Player.MaxHP -= _dropEquip.HP;
                Player.MaxSP -= _dropEquip.SP;
                Player.AttackPoint -= _dropEquip.Attack;
                Player.DefendPoint -= _dropEquip.Defend;
                Player.CritPoint -= _dropEquip.Crit;
                if (_dropEquip.Type == FitType.Weapon)
                    Player.Special = SpecialAbility.无;
                Player.Fit[(int)_dropEquip.Type] = 99;
            }
        }

        public void DressOnEquip(IEquip _dressOnEquip)
        {
            Player.MoveSpeed += _dressOnEquip.Speed / 10;
            Player.MaxHP += _dressOnEquip.HP;
            Player.MaxSP += _dressOnEquip.SP;
            Player.AttackPoint += _dressOnEquip.Attack;
            Player.DefendPoint += _dressOnEquip.Defend;
            Player.CritPoint += _dressOnEquip.Crit;
            if (_dressOnEquip.Type == FitType.Weapon)
                Player.Special = _dressOnEquip.Special;
        }
        
        public void ChangeProp(int _ID)
        {
            if (_ID >= Player.PropPack.Length || Player.PropPack[_ID] == null)
            {
                Debug.Log("不存在此道具!");
                return;
            }
            Player.Fit[(int)FitType.Medicine] = _ID;   // 更新Fit装备
        }

        public void DropProp()
        {
            Player.Fit[(int)FitType.Medicine] = 99;
        }
        /// <summary>
        /// 设置WeaponData，使用的是装备的武器
        /// 就目前的实现来说，所有Weapon对象均共用同一个IWeaponMono，所以初始化时关联一个默认Weapon，切换武器时切换Weapon对象即可
        /// </summary>
        /// <param name="_weapon">哪一个PlayerWeapon</param>
        private void UpdatePlayerWeapon(IPlayerWeapon _playerWeapon)
        {
            _playerWeapon.BasicAttack = Player.AttackPoint;
            _playerWeapon.Crit = Player.CritPoint;
            _playerWeapon.Special = Player.Special;
        }

    }
}
