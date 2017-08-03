using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    /// <summary>
    /// 单纯的显示Player的装备
    /// 变更装备时变更显示
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIFit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
    {
        [HideInInspector]
        public UIInventory uiInventory;
        [HideInInspector]
        public UIDragGrid grid;
        public int FitNum { get; set; } //0~7,7是特殊的
        private Image gridImg;  // grid的img
        private RectTransform rectTransform;
        private Image img;
        private Color normalColor=Color.white;  // 该物品格子的正常颜色
        private Color highLightColor = Color.red;   // 该物品格子的高亮颜色
        private bool hasItem = false;
        /// <summary>
        /// 由uiInventory进行初始化
        /// </summary>
        public void Initialize()
        {
            img = GetComponent<Image>();
            // 与grid子物体相互引用
            gridImg = transform.GetChild(0).GetComponent<Image>();
            rectTransform = this.transform as RectTransform;
        }

        private void DropFit()
        {
            // 卸下装备(如果有装备可卸的话)
            if (grid != null)
            {
                grid.E.gameObject.SetActive(false);
                grid.Selected = false;
                this.gridImg.sprite = null;
                this.gridImg.color = Color.clear;
            }
        }

        /// <summary>
        /// 切换显示的装备图像
        /// </summary>
        /// <param name="_slot"></param>
        public void ChangeFit(UIDragGrid _grid)
        {
            DropFit();
            if (_grid == null)
                return;
            this.hasItem = true;
            this.grid = _grid;
            if (_grid.IsEquip)
                this.gridImg.sprite = _grid.Equip.GetIcon();
            else
                this.gridImg.sprite = _grid.Prop.GetIcon();
            _grid.Selected = true;
            _grid.E.gameObject.SetActive(true);
            this.gridImg.color = Color.white;
        }

        /// <summary>
        /// 指针进入
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 显示详细信息
            if (hasItem)
            {
                img.color = highLightColor;
                uiInventory.ShowItemData(grid, transform.position);
            }
        }

        /// <summary>
        /// 指针离开
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            img.color = normalColor;
            uiInventory.CloseItemData();
        }

        /// <summary>
        /// 点击卸下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 武器不可卸下
            if (FitNum != (int)FitType.Weapon)
            {
                // View层
                DropFit();
                // Model层
                uiInventory.DropEquip(grid);
            }
        }

    }
}