using UnityEngine;


namespace SFramework
{
    /// <summary>
    /// Xml存档的数据对象
    /// </summary>
    public class GameData
    {
        //密钥,用于防止拷贝存档// 
        public string key;

        //下面是添加需要储存的内容，目前都是Player的内容// 
        // Player的属性由装备和初始值决定，无需记录
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Gold { get; set; }
        // Fit
        public int[] Fit;
        // Pack
        public IEquip[] EquipPack;
        public IProp[] PropPack;

        public GameData()
        {
            //构造函数，设置默认存档
            key = SystemInfo.deviceUniqueIdentifier;    //设定密钥，根据具体平台设定// 
            // Player
            Name = "Yuka";
            Rank = 1;
            Gold = 100;
            // 初始背包
            EquipPack = new IEquip[35];
            PropPack = new IProp[35];
            EquipPack[0] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicWeapon, "太刀");
            EquipPack[1] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicCloth, "学生服");
            EquipPack[2] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicShoe, "学生鞋");
            PropPack[0] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicMedicine, "回复药");
            // 初始装备
            Fit = new int[8] { 99, 99, 99, 99, 99, 99, 99, 99 }; // 初始值>34表示未装备
            Fit[(int)FitType.Weapon] = 0;
            Fit[(int)FitType.Cloth] = 1;
            Fit[(int)FitType.Shoe] = 2;
            Fit[(int)FitType.Medicine] = 0;
        }
    }
}
