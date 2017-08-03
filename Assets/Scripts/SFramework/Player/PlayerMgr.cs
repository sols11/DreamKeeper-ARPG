using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 角色控制系统
    /// 负责角色的创建(组件组装和初始化)，管理，删除
    /// </summary>
	public class PlayerMgr : IGameMgr
    {
        public IPlayer CurrentPlayer { get; private set; } //切换场景时不要消除引用
        public bool CanInput { get; set; }

        public PlayerMgr(GameMainProgram gameMain) : base(gameMain)
        {
        }

        public override void Initialize()
        {
            if (CurrentPlayer != null)
                CurrentPlayer.Initialize();
        }

        public override void Release()
        {
            // Player的销毁不由此执行
        }
        public override void Update()
        {
            if (CurrentPlayer != null && CanInput)
                CurrentPlayer.Update();
        }
        public override void FixedUpdate()
        {
            if (CurrentPlayer != null && CanInput)
                CurrentPlayer.FixedUpdate();
        }

        public void SetCurrentPlayer(IPlayer _player)
        {
            CurrentPlayer = _player;
            CurrentPlayer.Initialize();    // 设置Player时进行初始化
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

        public void SetPlayerPos(Vector3 _pos)
        {
            CurrentPlayer.GameObjectInScene.transform.position = _pos;
        }
    }
}
