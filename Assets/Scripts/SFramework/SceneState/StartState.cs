using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
	public class StartState : ISceneState
	{
		private GameMainProgram gameMainProgram;//主程序

		public StartState(SceneStateController Controller) : base(Controller)
		{
			this.StateName = "Begin";
		}

		public override void StateBegin()
		{
			gameMainProgram=GameMainProgram.Instance;
			gameMainProgram.Initialize();
            //UI初始化
            gameMainProgram.uiManager.ShowUIForms("BeginBackground");
            gameMainProgram.uiManager.ShowUIForms("ButtonPlay");

        }

        public override void StateEnd()
		{
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
