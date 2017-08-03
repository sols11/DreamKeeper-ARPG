using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIMainMenu : ViewBase
    {
        public Button battle;
        public Button pack;
        public Button store;
        public Button make;
        public Button setting;
        public Button help;
        public Button exit;
        private Text battleText;
        private Text packText;
        private Text storeText;
        private Text makeText;
        private Text settingText;
        private Text helpText;
        private Text exitText;

        void Awake()
        {
            //定义本窗体的性质
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            battleText = battle.transform.GetChild(0).GetComponent<Text>();
            packText = pack.transform.GetChild(0).GetComponent<Text>();
            storeText = store.transform.GetChild(0).GetComponent<Text>();
            makeText = make.transform.GetChild(0).GetComponent<Text>();
            settingText = setting.transform.GetChild(0).GetComponent<Text>();
            helpText = help.transform.GetChild(0).GetComponent<Text>();
            exitText = exit.transform.GetChild(0).GetComponent<Text>();
            battleText.text = base.ShowText("Battle");
            packText.text = base.ShowText("Pack");
            storeText.text = base.ShowText("Store");
            settingText.text = base.ShowText("Setting");
            helpText.text = base.ShowText("Help");
            exitText.text = base.ShowText("Exit");
        }

        void Start()
        {
            battle.onClick.AddListener(Battle);
            pack.onClick.AddListener(Pack);
            store.onClick.AddListener(Store);
            exit.onClick.AddListener(Exit);
        }

        private void Battle()
        {
            GameMainProgram.Instance.uiManager.ShowUIForms("BattleMenu");
        }

        private void Pack()
        {
            GameMainProgram.Instance.uiManager.ShowUIForms("Inventory");
        }

        private void Store()
        {
            GameMainProgram.Instance.uiManager.ShowUIForms("Store");
        }

        private void Exit()
        {
            GameLoop.Instance.sceneStateController.ExitGame();
        }

    }
}