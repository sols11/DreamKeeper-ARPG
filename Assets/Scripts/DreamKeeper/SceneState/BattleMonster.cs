using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class BattleMonster : ISceneState
    {
        private GameMainProgram gameMainProgram;//主程序

        public BattleMonster(SceneStateController Controller) : base(Controller)
		{
            this.StateName = "BattleTest";
        }

        public override void StateBegin()
        {
            gameMainProgram = GameMainProgram.Instance;
            gameMainProgram.Initialize();
            // 启用Mgr
            gameMainProgram.courseMgr.Enable = true;
            //场景初始化
            gameMainProgram.playerMgr.BuildPlayer(Vector3.zero);
            gameMainProgram.playerMgr.playerYuka.BackIdle();
            gameMainProgram.playerMgr.CanInput = true;  // 接受输入
            gameMainProgram.enemyMgr.CreateMonster(new Vector3(0, 0, 7));
            //UI在后面初始化
            gameMainProgram.uiManager.ShowUIForms("PlayerHUD");
            gameMainProgram.uiManager.ShowUIForms("MedicineHUD");

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