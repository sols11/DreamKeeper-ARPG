using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamKeeper;

namespace SFramework
{
    /// <summary>
    /// 角色控制系统
    /// 负责角色的创建(组件组装和初始化)，管理，删除
    /// </summary>
	public class PlayerMgr : IGameMgr
	{
		private GameObject playerBuild; // 用于创建角色

        public IPlayer CurrentPlayer { get; private set; } //切换场景时不要消除引用
        public PlayerYuka playerYuka;   // DK使用的主角
        public bool CanInput { get; set; }

        public PlayerMgr(GameMainProgram gameMain):base(gameMain)
		{			
		}

        public override void Initialize()
        {
            if (CurrentPlayer != null)
                CurrentPlayer.Initialize();
        }

        public override void Release() {
            // Player的销毁不由此执行
		}
		public override void Update() {
            if(CurrentPlayer!=null&& CanInput)
                CurrentPlayer.Update();
		}
		public override void FixedUpdate() {
            if(CurrentPlayer!=null&& CanInput)
                CurrentPlayer.FixedUpdate();
		}

        /// <summary>
        /// 若Player已存在，则只是移动Pos
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
		public void BuildPlayer(Vector3 _pos)
		{
            if (CurrentPlayer == null)
            {
                //GameObject.Destroy(GameObject.FindGameObjectWithTag("Player"));
                playerBuild = GameMainProgram.Instance.resourcesMgr.LoadAsset(@"Players\Kashima", false, _pos,Quaternion.identity);
                playerYuka = new PlayerYuka(playerBuild);
                CurrentPlayer = playerYuka;
                CurrentPlayer.Initialize();
            }
                playerBuild.transform.position = _pos;
        }

        public void DestroyPlayer()
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.Release();
                GameObject.Destroy(CurrentPlayer.GameObjectInScene);
            }
            else
                Debug.Log("无CurrentPlayer可以销毁！");
        }
    }
}
