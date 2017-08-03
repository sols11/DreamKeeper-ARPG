using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class MainScene : ISceneState
    {
        private GameMainProgram gameMainProgram;//主程序

        public MainScene(SceneStateController Controller) : base(Controller)
        {
            this.StateName = "Main";
        }

        public override void StateBegin()
        {
            gameMainProgram = GameMainProgram.Instance;
            gameMainProgram.Initialize();
            gameMainProgram.courseMgr.Enable = false;
            //Test
            gameMainProgram.playerMgr.BuildPlayer(new Vector3(-0.08f,0,-1));
            gameMainProgram.playerMgr.CanInput = false;  // 禁止输入
            gameMainProgram.playerMgr.playerYuka.MainIdle();
            //UI初始化
            gameMainProgram.uiManager.ShowUIForms("MainMenu");


        }

        public override void StateEnd()
        {
            // State切换时，GameMainProgram及其各个Mgr并没有销毁，而是调用Release，然后重新Initialize
            gameMainProgram.Release();
        }

        public override void StateUpdate()
        {
            gameMainProgram.Update();
        }

        public override void FixedUpdate()
        {
            gameMainProgram.FixedUpdate();
        }

    }
}