using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIPlayerHUD : ViewBase
    {
        public RectTransform uiHP;
        public RectTransform uiSP;
        public Text textLevel;
        [SerializeField]
        private float minWidthHP = 0;
        [SerializeField]
        private float maxWidthHP = 100;
        [SerializeField]
        private float minWidthSP = 0;
        [SerializeField]
        private float maxWidthSP = 100;
        private int playerLevel = 1;

        private IPlayer player;

        void Awake()
        {
            //定义本窗体的性质(默认数值，可以不写)
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
        }

        void Start()
        {
            // 观察者注册
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer;
            GameMainProgram.Instance.eventMgr.StartListening(EventName.PlayerHP_SP, this.UpdateUI);
            UpdateUI(); // 进行初始化
        }

        void OnDestroy()
        {
            GameMainProgram.Instance.eventMgr.StopListening(EventName.PlayerHP_SP, this.UpdateUI);
        }

        public override void UpdateUI()
        {
            // 计算出fillAmount
            if (uiHP != null)
                uiHP.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, Mathf.Round(minWidthHP + ((maxWidthHP - minWidthHP) * player.HPpercent)));
            if (uiSP != null)
                uiSP.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, Mathf.Round(minWidthSP + ((maxWidthSP - minWidthSP) * player.SPpercent)));

        }
    }
}
