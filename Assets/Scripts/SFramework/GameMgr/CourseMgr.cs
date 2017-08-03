using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 对游戏的进程进行分析，控制
    /// 功能：暂停游戏
    /// </summary>
    public class CourseMgr : IGameMgr
    {
        /// <summary>
        /// 是否启用该系统
        /// </summary>
        public bool Enable { get; set; }

        private bool pauseGame = false;
        private int defaultTimeScale = 1;

        public CourseMgr(GameMainProgram gameMain):base(gameMain)
		{

        }

        public override void Initialize()
        {
            Enable = false;
        }

        public override void Release()
        {
            if (!Enable)
                return;
            if (pauseGame) // 解除暂停
                PauseGame();
        }

        public override void Update()
        {
            if(!Enable)
                return;
            if (Input.GetButtonDown("Cancel"))
                PauseGame();
        }

        public void PauseGame()
        {
            if (!Enable)
            {
                Debug.LogWarning("未开启CourseMgr");
                return;
            }
            if (!pauseGame)
            {
                pauseGame = true;
                GameMainProgram.Instance.uiManager.ShowUIForms("PauseMenu");
                if (Time.timeScale != 0)
                    Time.timeScale = 0;
                else
                    Debug.LogWarning("已经暂停，请检查");
            }
            else
            {
                pauseGame = false;
                GameMainProgram.Instance.uiManager.CloseUIForms("PauseMenu");
                if (Time.timeScale==0)
                    Time.timeScale = defaultTimeScale;
                else
                    Debug.LogWarning("游戏并未暂停，请检查");
            }
        }

    }
}