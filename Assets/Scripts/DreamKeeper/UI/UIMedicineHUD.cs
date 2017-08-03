using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIMedicineHUD : ViewBase
    {
        public Text textNum;
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
            GameMainProgram.Instance.eventMgr.StartListening(EventName.MedicineNum, this.UpdateUI);
            UpdateUI(); // 进行初始化
        }

        void OnDestroy()
        {
            GameMainProgram.Instance.eventMgr.StopListening(EventName.MedicineNum, this.UpdateUI);
        }

        public override void UpdateUI()
        {
            textNum.text = player.MedicineNum.ToString();
        }

    }
}