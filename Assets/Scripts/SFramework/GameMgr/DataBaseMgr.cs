using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 游戏数据库
    /// </summary>
    public class DataBaseMgr : IGameMgr
    {
        private string dataBasePath = Application.streamingAssetsPath + @"\DataBase\";
        // 字典存放所有种类的Item，key也可以用enum
        public Dictionary<string, IEquip> dicWeapon;
        public Dictionary<string, IEquip> dicCloth;
        public Dictionary<string, IEquip> dicShoe;
        public Dictionary<string, IProp> dicMedicine;
        public Dictionary<string, IEquip> dicEnemyWeapon;
        public Dictionary<string, IEquip> dicEnemyCloth;

        public DataBaseMgr(GameMainProgram gameMain) : base(gameMain)
        {
            dicWeapon = new Dictionary<string, IEquip>();
            dicCloth = new Dictionary<string, IEquip>();
            dicShoe = new Dictionary<string, IEquip>();
            dicMedicine = new Dictionary<string, IProp>();
            dicEnemyWeapon = new Dictionary<string, IEquip>();
            dicEnemyCloth = new Dictionary<string, IEquip>();
        }

        public override void Awake()
        {
            dicWeapon = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IEquip>>("Weapon");
            dicCloth = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IEquip>>("Cloth");
            dicShoe = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IEquip>>("Shoe");
            dicMedicine = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IProp>>("Medicine");
            dicEnemyWeapon = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IEquip>>("EnemyWeapon");
            dicEnemyCloth = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, IEquip>>("EnemyCloth");
        }

        private void GenerateDic()
        {
            gameMain.fileMgr.CreateJsonDataBase("Test", dicWeapon);
        }

    }
}