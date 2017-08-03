using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFramework;

namespace DreamKeeper
{
    /// <summary>
    /// 商店的物品信息，用Json存档保存
    /// 请在修改信息后保存
    /// </summary>
    public class StoreData
    {
        public class EquipPair
        {
            public FitType Type;
            public string Key;
        }
        public class PropPair
        {
            public PropType Type;
            public string Key;
        }

        // 因为要存放两个元素所以用Pair,不用KeyValuePair是因为要求只读
        public EquipPair[] EquipKeys = new EquipPair[35];
        public PropPair[] PropKeys = new PropPair[35];

        public StoreData()
        {
            // 默认存档
            SetEquipData(0, FitType.Weapon, "菜刀");
            SetEquipData(1, FitType.Weapon, "青铜刀");
            SetEquipData(2, FitType.Cloth, "布衣");
            SetEquipData(3, FitType.Shoe, "童鞋");
            SetPropData(0, PropType.Medicine, "回复药");

        }

        // 修改和移除商店物品的操作

        public void SetEquipData(int _ID,FitType _Type,string _Key)
        {
            if (EquipKeys[_ID] == null)
                EquipKeys[_ID] = new EquipPair();
            EquipKeys[_ID].Type = _Type;
            EquipKeys[_ID].Key = _Key;
        }

        public void ClearEquipData(int _ID)
        {
            EquipKeys[_ID] = null;
        }

        public void SetPropData(int _ID, PropType _Type, string _Key)
        {
            if (PropKeys[_ID] == null)
                PropKeys[_ID] = new PropPair();
            PropKeys[_ID].Type = _Type;
            PropKeys[_ID].Key = _Key;
        }

        public void ClearPropData(int _ID)
        {
            PropKeys[_ID] = null;
        }
    }
}
