using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIBattleMenu : ViewBase
    {
        [SerializeField]
        private RectTransform Mission;
        [SerializeField]
        private Button ConfirmBtn;
        [SerializeField]
        private Button CloseBtn;

        private Button[] NoBtn=new Button[6];

        void Awake()
        {
            //定义本窗体的性质(默认数值，可以不写)
            base.UIForm_Type = UIFormType.Fixed;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            int i = 0;
            foreach (RectTransform r in Mission)
                NoBtn[i++] = r.GetComponent<Button>();
        }

        void Start()
        {
            CloseBtn.onClick.AddListener(Close);
            ConfirmBtn.onClick.AddListener(Confirm);
        }

        private void Confirm()
        {
            Close();
            // 切换Scene
            GameLoop.Instance.sceneStateController.SetState(new BattleMonster(GameLoop.Instance.sceneStateController), true,true);
        }

        private void Close()
        {
            GameMainProgram.Instance.uiManager.CloseUIForms("BattleMenu");
        }
    }
}