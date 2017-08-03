using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIBtnPlay : ViewBase
    {
        public Button btn;

        void Awake()
        {
            //定义本窗体的性质(默认数值，可以不写)
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
        }

        void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            // 切换Scene
            GameLoop.Instance.sceneStateController.SetState(new MainScene(GameLoop.Instance.sceneStateController), true);
        }

    }
}