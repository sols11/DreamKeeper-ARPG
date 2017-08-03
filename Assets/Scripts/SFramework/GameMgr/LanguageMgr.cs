using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 多语言本地化
    /// </summary>
    public class LanguageMgr:IGameMgr
    {
        private Dictionary<string, string> dicLauguageCache;

        public LanguageMgr(GameMainProgram gameMain):base(gameMain){
            dicLauguageCache = new Dictionary<string, string>();

        }

        public override void Awake()
        {
            LoadLanguageCache();
        }

        /// <summary>
        /// 到显示文本信息
        /// </summary>
        /// <param name="lauguageID">语言的ID</param>
        /// <returns></returns>
        public string ShowText(string stringID)
        {
            return UnityHelper.FindDic(dicLauguageCache, stringID);
        }

        private void CreateLanguageCache()
        {
            gameMain.fileMgr.CreateJsonDataBase("Language_CN", dicLauguageCache);
        }
        private void LoadLanguageCache()
        {
            dicLauguageCache=gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, string>>("Language_CN");
        }
    }
}
