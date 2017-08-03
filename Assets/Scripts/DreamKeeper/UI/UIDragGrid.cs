using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SFramework;

namespace DreamKeeper
{
    /// <summary>
    /// 控制整个Slot，是背包的组成部分
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIDragGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static Image dragImg = null;  // 拖拽时使用的临时Image，所有对象共用
        [HideInInspector]
        public UIInventory uiInventory;
        /// <summary>
        /// ID 对应了在EquipPack和EquipArray的索引，表示了在所有Grid中的位置
        /// </summary>
        public int ID { get; set; }
        public Transform E { get; private set; }
        private Text count;
        // 存储信息引用
        public bool HasItem { get; set; }   // 目前是否有存放物品（武器或道具）
        public bool Selected { get; set; }    // 该格子的物品是否装备了
        public bool IsEquip { get; set; }
        public IEquip Equip { get; set; }
        public IProp Prop { get; set; }

        private Image img; // Image显示
        private Image parentImg;
        // ui的RectTransform，转换屏幕坐标用
        private RectTransform uiRectTransform;
        // 拖拽操作前的有效位置，拖拽到有效位置时更新
        private Vector3 originalPosition;

        // Slot的正常颜色
        private Color normalColor;
        // 拖拽至新的物品格子时，Slot的高亮颜色
        private Color highLightColor = Color.red;
        private Vector3 offset = new Vector3();    //用来得到鼠标和图片的差值
        private Vector3 globalMousePos; //因为使用的是WorldPoint所以用3维

        public void Initialize()
        {
            //gameObject.name = "Grid"; // name 必须是Grid，这样能保证挂载了该脚本
            count = transform.parent.GetChild(1).GetComponent<Text>();
            E = transform.parent.GetChild(2);
            E.gameObject.SetActive(false);
            parentImg = transform.parent.GetComponent<Image>();
            normalColor = parentImg.color;

            uiRectTransform = this.transform as RectTransform;
            originalPosition = transform.position;
            img = GetComponent<Image>();
        }
        /// <summary>
        /// 指针点击：装备。调用背包
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 成功装备则显示E
            if (HasItem)
                uiInventory.SelectItem(this);
        }
        /// <summary>
        /// 指针进入：高亮，显示详细信息。调用背包
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            parentImg.color = highLightColor;
            // 显示详细信息
            if (HasItem)
                uiInventory.ShowItemData(this, transform.position);
        }
        /// <summary>
        /// 指针离开：关闭详细信息
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            parentImg.color = normalColor;
            uiInventory.CloseItemData();
        }

        /// <summary>
        /// 开始拖拽：记录位置，与dragImg交换Img，计算世界坐标下的UI和鼠标点的差值
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = transform.position;//拖拽前记录起始位置
            if (img.sprite == null) // 如果是空格子那么不继续执行
                return;
            // 与dragImg交换Img
            dragImg.gameObject.SetActive(true);
            dragImg.rectTransform.position = originalPosition;
            Sprite tempSprite = dragImg.sprite;
            Color tempColor = dragImg.color;
            dragImg.sprite = img.sprite;
            dragImg.color = img.color;
            img.sprite = tempSprite;
            img.color = tempColor;
            bool isRect = RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRectTransform, eventData.position, eventData.enterEventCamera, out globalMousePos);
            if (isRect)   //如果在
            {
                //计算世界坐标下的UI和鼠标点的差值
                offset = uiRectTransform.position - globalMousePos;
            }
            else
            {
                offset = Vector3.zero;
            }
        }
        /// <summary>
        /// 拖拽中：移动位置，保持偏移
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            // 移动位置，保持偏移
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
                dragImg.rectTransform.position = globalMousePos+ offset;
        }
        /// <summary>
        /// 拖拽结束：让临时Image回到原始状态。如果要互换，需要交换Grid信息,物品在背包中的ID，Fit对应的ID，然后再更新显示
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            GameObject curPointerEnter = eventData.pointerEnter;
            Sprite tempSprite = null;
            Color tempColor;
            // DragImg和Img交换回来，无论是否与其他格子交换了
            tempSprite = dragImg.sprite;
            tempColor = dragImg.color;
            dragImg.sprite = img.sprite;
            dragImg.color = img.color;
            img.sprite = tempSprite;
            img.color = tempColor;
            //
            dragImg.gameObject.SetActive(false);
            //移动至物品格子上,且不能是和自身交换
            if (curPointerEnter == null)
                return;
            if (curPointerEnter.name == "Grid" && curPointerEnter != gameObject) 
            {
                UIDragGrid otherGrid = curPointerEnter.GetComponent<UIDragGrid>();
                if (otherGrid)
                {
                    // 拖拽交换。只需要交换Grid信息,物品在背包中的ID，Fit对应的ID，然后再更新
                    if (IsEquip)
                        uiInventory.ExchangeEquip(this, otherGrid);
                    else
                        uiInventory.ExchangeProp(this, otherGrid);

                    GridExchange(otherGrid);
                    // 交换grid后因为Fit可能关联了grid所以要更新
                    if (IsEquip)
                        uiInventory.ExchangeFitEquip(this, otherGrid);
                    else
                        uiInventory.ExchangeFitProp(this, otherGrid);
                    // 更新显示
                    UpdateImage();
                    otherGrid.UpdateImage();
                }

            }
        }

        /// <summary>
        /// 交换Grid的信息
        /// </summary>
        /// <param name="_otherGrid"></param>
        private void GridExchange(UIDragGrid _otherGrid)
        {
            bool _HasItem = _otherGrid.HasItem;
            _otherGrid.HasItem = HasItem;
            HasItem = _HasItem;
            bool _Selected = _otherGrid.Selected;
            _otherGrid.Selected = Selected;
            Selected = _Selected;
            IEquip _Equip = _otherGrid.Equip;
            _otherGrid.Equip = Equip;
            Equip = _Equip;
            IProp _Prop = _otherGrid.Prop;
            _otherGrid.Prop = Prop;
            Prop = _Prop;
        }
        /// <summary>
        /// 更新显示的UI
        /// </summary>
        public void UpdateImage()
        {
            E.gameObject.SetActive(Selected);
            if (IsEquip)
            {
                // 是否存在显示不同
                if (Equip!=null)
                {
                    img.sprite = Equip.GetIcon();
                    img.color = Color.white;
                    HasItem = true;
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    HasItem = false;
                }
            }
            else
            {
                // 是否存在显示不同
                if (Prop!=null)
                {
                    img.sprite = Prop.GetIcon();
                    img.color = Color.white;
                    HasItem = true;
                    count.text = Prop.Num.ToString();
                    count.gameObject.SetActive(true);
                }
                else
                {
                    img.sprite = null;
                    img.color = Color.black;
                    HasItem = false;
                    count.gameObject.SetActive(false);
                }
            }
        }
    }
}