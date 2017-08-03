//========================================================================================================= 
//Note: Data Managing. 
//Date Created: 2012/04/17 by 风宇冲 
//Date Modified: 2012/12/14 by 风宇冲 
//========================================================================================================= 
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

namespace SFramework
{
    //管理数据储存的类// 
    public class GameDataMgr : IGameMgr
    {
        private string dataFileName = "DreamKeeperSave.xml";//存档文件的名称,自己定// 
        private XmlSaver xs;

        public GameData gameData;//实例

        public GameDataMgr(GameMainProgram gameMain) : base(gameMain)
        {
            xs = new XmlSaver();
        }

        public override void Awake()
        {
            //gameData = new GameData(); // 创建初始存档
        }
        // Json用存档
        public void SaveJson()
        {
            gameMain.fileMgr.CreateJsonSaveData(GetDataPath() + "/Save", gameData);
        }
        public GameData LoadJson()
        {
            gameData=gameMain.fileMgr.LoadJsonSaveData<GameData>(GetDataPath() + "/Save");
            if(gameData==null)
            {
                Debug.Log("未能找到存档，自动创建");
                gameData = new GameData(); // 创建初始存档
                SaveJson();
            }
            else
                Debug.Log("已读取存档");

            return gameData;
        }

        //存档时调用的函数// 
        public void Save()
        {
            string gameDataFile = GetDataPath() + "/" + dataFileName;
            Debug.Log("存档已保存于" + gameDataFile);
            //将存档写入XML文件
            string dataString = xs.SerializeObject(gameData, typeof(GameData));
            xs.CreateXML(gameDataFile, dataString);
        }

        //读档时调用的函数,游戏开始时Player自动读档，如果没有存档那么自动创建// 
        public GameData Load()
        {
            string gameDataFile = GetDataPath() + "/" + dataFileName;
            if (xs.hasFile(gameDataFile))
            {
                string dataString = xs.LoadXML(gameDataFile);
                GameData gameDataFromXML = xs.DeserializeObject(dataString, typeof(GameData)) as GameData;

                //是合法存档// 
                if (gameDataFromXML.key == SystemInfo.deviceUniqueIdentifier)
                {//将存档赋给当前实例
                    Debug.Log("已读取存档");
                    gameData = gameDataFromXML;
                    return gameData;
                }
                //是非法拷贝存档// 
                    //留空：游戏启动后数据清零，存档后作弊档被自动覆盖// 
            }

                gameData = new GameData(); // 创建初始存档
                Save();
            return gameData;
        }

        //获取路径// 
        private static string GetDataPath()
        {
            // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
            // Application.dataPath returns ar/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data             
            // Strip "/Data" from path 
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Iphone");
                string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                // Strip application name 
                path = path.Substring(0, path.LastIndexOf('/'));
                return path + "/Documents";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("android");
                string path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return path;
            }
            else
                // 这里先用着dataPath，游戏做成了换成persistentDataPath
                return Application.dataPath;
        }
    }
}