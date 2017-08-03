using System.Collections;
using SFramework;
using UnityEngine;
using UnityEngine.UI;

namespace DreamKeeper
{
    public class UIPromptBox : ViewBase
    {
        public Text word;
        public Button btn;
        public static int wordID = 0;   // 修改显示内容

        private void Awake()
        {
            base.UIForm_Type = UIFormType.PopUp;
            base.UIForm_ShowMode = UIFormShowMode.ReverseChange;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;

            btn.onClick.AddListener(Close);
        }

        private void OnEnable()
        {
            switch(wordID)
            {
                case 1:
                    word.text = "卸下装备后才可出售";
                    break;
                case 2:
                    word.text = "背包物品已满";
                    break;
                case 3:
                    word.text = "装备背包已满";
                    break;
                case 4:
                    word.text = "你的金币不够";
                    break;
                case 5:
                    word.text = "请先选中物品";
                    break;
                case 6:
                    word.text = "该物品不可出售";
                    break;
                default:
                    break;
            }
        }

        private void Close()
        {
            GameMainProgram.Instance.uiManager.CloseUIForms("PromptBox");
        }

    }
}