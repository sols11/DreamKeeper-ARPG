using UnityEngine;
using System;
using System.Collections;
using DreamKeeper;

namespace SFramework
{
	/// <summary>
	/// 游戏主循环
	/// </summary>
	public class GameLoop : MonoBehaviour
	{
        private static GameLoop _instance;

        public static GameLoop Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameLoop>();

                    if (_instance == null)
                        _instance = new GameObject("GameLoop").AddComponent<GameLoop>();
                }
                return _instance;
            }
        }

        public SceneStateController sceneStateController = new SceneStateController();

        void Awake()
        {
            if (_instance == null)
                _instance = GetComponent<GameLoop>();
            else if (_instance != GetComponent<GameLoop>())
            {
                Debug.LogWarningFormat("There is more than one {0} in the scene，auto inactive the copy one.", typeof(GameLoop).ToString());
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

		void Start()
		{
			// 設定起始的場景，因为起始场景一般是Unity发布时决定，所以不需要写SceneName，只要赋引用就行了
			sceneStateController.SetState(new MainScene(sceneStateController), false);
		}

		void Update()
		{
			sceneStateController.StateUpdate();
		}

		void FixedUpdate()
		{
			//物理相关的处理
			sceneStateController.FixedUpdate();
		}
	}
}
