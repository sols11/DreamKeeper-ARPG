using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamKeeper;

namespace SFramework
{
    /// <summary>
    /// 敌人控制系统
    /// 负责敌人的创建，管理，删除
    /// 存储整个场景中的所有Enemy
    /// </summary>
	public class EnemyMgr : IGameMgr {

		private List<IEnemy> enemysInScene;
		private IEnemy newEnemy; 

		public EnemyMgr(GameMainProgram gameMain):base(gameMain)
		{
            enemysInScene = new List<IEnemy>();
		}

		public override void Initialize()
		{
            gameMain.eventMgr.StartListening(EventName.PlayerDead, NotifyPlayerDead);
		}
		public override void Release()
		{
			foreach (IEnemy e in enemysInScene)
				e.Release();
            enemysInScene.Clear();
            gameMain.eventMgr.StopListening(EventName.PlayerDead, NotifyPlayerDead);
        }
		public override void Update()
		{
			foreach (IEnemy e in enemysInScene)
				e.Update();
		}
		public override void FixedUpdate()
		{
			foreach (IEnemy e in enemysInScene)
				e.FixedUpdate();
		}
        public void NotifyPlayerDead()
        {
            foreach (IEnemy e in enemysInScene)
                e.WhenPlayerDead();
        }

		public void CreateMonster(Vector3 _pos)
		{
			newEnemy =new EnemyMonster(GameMainProgram.Instance.resourcesMgr.LoadAsset(@"Enemys\Monster", false, _pos,Quaternion.Euler(0,-180,0)));
			//协变，会根据new的类型创建一个对象
			if (newEnemy != null)
			{
                enemysInScene.Add(newEnemy);
                newEnemy.Initialize();
			}
		}
        public void CreateRoyalKnight(Vector3 _pos)
        {
			newEnemy =new EnemyKnight(GameMainProgram.Instance.resourcesMgr.LoadAsset(@"Enemys\RoyalKnight", false, _pos, Quaternion.Euler(0, -180, 0)));
            //协变，会根据new的类型创建一个对象
            if (newEnemy != null)
            {
                enemysInScene.Add(newEnemy);
                newEnemy.Initialize();
            }
        }
    }
}