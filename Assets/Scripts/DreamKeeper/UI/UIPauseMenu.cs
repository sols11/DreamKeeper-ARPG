using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIPauseMenu : ViewBase
    {
        [SerializeField]
        private Button resume;
        [SerializeField]
        private Button restart;
        [SerializeField]
        private Button exit;

        void Awake()
        {
            //定义本窗体的性质(弹出窗体)
            base.UIForm_Type = UIFormType.PopUp;
            base.UIForm_ShowMode = UIFormShowMode.ReverseChange;
            base.UIForm_LucencyType = UIFormLucenyType.Translucence;
        }

        void Start()
        {
            resume.onClick.AddListener(Resume);
            restart.onClick.AddListener(Restart);
            exit.onClick.AddListener(Exit);

        }

        private void Resume()
        {
            GameMainProgram.Instance.courseMgr.PauseGame();
        }

        private void Restart()
        {
            Resume();
            GameLoop.Instance.sceneStateController.SetState(GameLoop.Instance.sceneStateController.CurState);
        }

        private void Exit()
        {
            Resume();
            GameLoop.Instance.sceneStateController.ExitGame();
        }

    }
}