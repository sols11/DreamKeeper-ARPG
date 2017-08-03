using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIStoreGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [HideInInspector]
        public UIStore uiStore;

        public int ID { get; set; }
        public bool IsEquip { get; set; }   // 可存装备或道具，放于不同的Slots
        public IEquip Equip { get; set; }
        public IProp Prop { get; set; }
        public bool IsLocked { get; set; }    // 该格子的物品是否被选中
        public Transform E { get; private set; }    // 表示是否装备
        public Text Price { get; set; }

        private Image img; 
        private Image parentImg;
        // Slot的正常颜色
        private static Color normalColor = new Color(0.55f,0.55f,0.55f,1);
        // Slot的高亮颜色
        private static Color highLightColor = Color.red;
        // Slot的锁定颜色
        private static Color lockColor = Color.green;
        // price的通常颜色
        private static Color priceColor = Color.white; //new Color(0.77f, 0.77f, 0.77f, 1);
        // price的警告颜色
        private static Color priceWarnColor = Color.red;

        /// <summary>
        /// 由UIStore进行初始化
        /// </summary>
        public void Initialize()
        {
            img = GetComponent<Image>();
            parentImg = transform.parent.GetComponent<Image>();
            Price= transform.parent.GetChild(1).GetComponent<Text>();
            E = transform.parent.GetChild(2);
            Price.color = priceColor;
            Price.gameObject.SetActive(false);
        }

        /// <summary>
        /// 再次点击/点击其他/点击按钮都会解锁，由UIStore调用
        /// </summary>
        public void UnLock()
        {
            IsLocked = false;
            parentImg.color = normalColor;
        }

        /// <summary>
        /// 指针点击：锁定
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 解锁
            if(IsLocked)
            {
                uiStore.UnLock(); // Store
                return;
            }
            // 锁定
            if (IsEquip && Equip != null)
            {
                IsLocked = true;
                uiStore.LockEquip(ID);
                parentImg.color = lockColor;
            }
            else if (!IsEquip && Prop != null)
            {
                IsLocked = true;
                uiStore.LockProp(ID);
                parentImg.color = lockColor;
            }
        }
        /// <summary>
        /// 指针进入：高亮，显示详细信息。
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsLocked)
                parentImg.color = highLightColor;
            // 显示详细信息
            if (IsEquip && Equip != null)
                uiStore.ShowItemData(this, transform.position);
            else if (!IsEquip && Prop != null)
                uiStore.ShowItemData(this, transform.position);
        }
        /// <summary>
        /// 指针离开：关闭详细信息
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsLocked)
                parentImg.color = normalColor;
            uiStore.CloseItemData();
        }

        /// <summary>
        /// 卖出当前物品
        /// </summary>
        /// <returns></returns>
        public bool SaleItem()
        {
            if (IsEquip)
            {
                if (Equip == null)
                {
                    UIPromptBox.wordID = 5;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                if (!Equip.CanSale)
                {
                    UIPromptBox.wordID = 6;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                if (E.gameObject.activeSelf)
                {
                    UIPromptBox.wordID = 1;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                Equip = null;
                UpdateSale();
                return true;
            }
            else
            {
                if (Prop == null || Prop.Num <= 0)
                {
                    Prop = null;
                    UIPromptBox.wordID = 5;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                if (!Prop.CanSale)
                {
                    UIPromptBox.wordID = 6;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                if (E.gameObject.activeSelf)
                {
                    UIPromptBox.wordID = 1;
                    GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    return false;
                }
                Prop.Num -= 1;
                if (Prop.Num <= 0)
                    Prop = null;
                UpdateSale();
                Price.text = Prop.Num.ToString();    //显示更新
                return true;
            }
        }

        /// <summary>
        /// 更新显示的UI:Image和Price
        /// </summary>
        public void UpdateBuy()
        {
            // 先全部关掉E
            if (E.gameObject.activeSelf)
                E.gameObject.SetActive(false);
            if (IsEquip)
            {
                // 是否存在显示不同
                if (Equip != null)
                {
                    img.sprite = Equip.GetIcon();
                    img.color = Color.white;
                    Price.text = Equip.Price.ToString();    //价格
                    Price.gameObject.SetActive(true); 
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    Price.gameObject.SetActive(false);
                }
            }
            else
            {
                // 是否存在显示不同
                if (Prop != null)
                {
                    img.sprite = Prop.GetIcon();
                    img.color = Color.white;
                    Price.text = Prop.Price.ToString();    //价格
                    Price.gameObject.SetActive(true);
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    Price.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 更新显示的UI:Image,E和SalePrice
        /// </summary>
        public void UpdateSale()
        {
            // 先全部关掉E
            if(E.gameObject.activeSelf)
                E.gameObject.SetActive(false);
            if (IsEquip)
            {
                // 是否存在显示不同
                if (Equip != null)
                {
                    img.sprite = Equip.GetIcon();
                    img.color = Color.white;
                    Price.gameObject.SetActive(false);
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    Price.gameObject.SetActive(false);
                }
            }
            else
            {
                // 是否存在显示不同
                if (Prop != null)
                {
                    img.sprite = Prop.GetIcon();
                    img.color = Color.white;
                    Price.text = Prop.Num.ToString();    //数目
                    Price.gameObject.SetActive(true);
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    Price.gameObject.SetActive(false);
                }
            }
        }

    }
}