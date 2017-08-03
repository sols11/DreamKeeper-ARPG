using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using PigeonCoopToolkit.Effects.Trails;

namespace DreamKeeper
{
	/// <summary>
	/// katana的动画事件函数
	/// </summary>
	public class KatanaAnimEvent : IPlayerMono
	{
        private PlayerYuka playerYuka; // 对应的IPlayer
		private Trail trail;
        private string medicineGreenPath;

        /// <summary>
        /// 由IPlayer调用
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            medicineGreenPath = @"PlayerSkills\MedicineGreen";
            if (iPlayerWeapon != null)
                trail = iPlayerWeapon.GetComponentInChildren<Trail>();
            else
                Debug.LogError("iPlayerWeapon未赋值");
            playerYuka = PlayerMedi.Player as PlayerYuka;
        }

        /// <summary>
        /// 用于动画调用开关轨迹
        /// </summary>
        /// <param name="v"></param> 
        public override void TrailSwitch(int open)
		{
			if(open==3)//只清空list
			{
                iPlayerWeapon.ClearList();
			}
			else if (open==2)//只开启特效
			{
				trail.Emit = true;
			}
			else if (open == 1)//开启 特效+碰撞
			{
                PlaySound(0);
                trail.Emit = true;
				WeaponCollider.enabled = true;
                iPlayerWeapon.ClearList();
			}
			else //关闭 特效+碰撞
			{
				trail.Emit = false;
				WeaponCollider.enabled = false;
			}
		}

        /// <summary>
        /// 回复HP
        /// </summary>
        public void DrinkMedicine()
        {
            playerYuka.DrinkMedicine();
            GameMainProgram.Instance.resourcesMgr.LoadAsset(medicineGreenPath, true, transform.position, Quaternion.identity);
        }
        /// <summary>
        /// 判断是否是重攻击
        /// </summary>
        public bool IsHeavyAttack()
        {
            stateInfo = AnimatorComponent.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("2Att1") || stateInfo.IsName("2Att2") || stateInfo.IsName("2Att3") ||
                stateInfo.IsName("2Att4") || stateInfo.IsName("3Att2") || stateInfo.IsName("3Att5"))
                return true;                        
            return false;
        }
        /// <summary>
        /// 提供给WeaponKatana调用
        /// </summary>
        public void Parried()
        {
            playerYuka.Parried();
            TrailSwitch(0); // 关闭武器
            velocityForward(-8); // 后退
        }


    }
}