using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace DreamKeeper
{
    /// <summary>
    /// 背包UI，管理其下的所有UI
    /// 获取数据，接受操作，保存数据
    /// </summary>
    public class UIInventory : ViewBase
    {
        #region Variables
        public Text Name;
        public Text Rank;
        public Text Spec;
        public Text HP;
        public Text SP;
        public Text ATT;
        public Text DEF;
        public Text CRI;
        public Text SPD;
        public RectTransform Fit;

        public Button EquipBtn;
        public Button PropBtn;
        public RectTransform EquipSlots;
        public RectTransform PropSlots;
        private UIFit[] Fits = new UIFit[8];    // grid[]
        public Text Gold;

        public Image DragImg;
        public Image ItemData;
        public Text ItemName;
        public Text ItemDetail;
        public Button CloseBtn;

        private UIDragGrid[] EquipArray = new UIDragGrid[35]; //装备数组，装备界面下的格子存储顺序
        private UIDragGrid[] PropArray = new UIDragGrid[35];    //道具数组
        private Text EquipBtnText;
        private Text PropBtnText;
        private Color EquipBtnColor;
        private Color OrangeTextColor;
        private Color GreyTextColor;
        private int Speed; // SPD计算转换
        private IPlayer player;
        #endregion

        /// <summary>
        /// Awake：初始化所有子物体。第一次打开时从Player获取背包信息，然后更新装备
        /// </summary>
        private void Awake()
        {
            base.UIForm_Type = UIFormType.PopUp;
            base.UIForm_ShowMode = UIFormShowMode.ReverseChange;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer;
            // 由本类给子物体初始化
            Initialize();

            EquipBtn.onClick.AddListener(ButtonEquip);
            PropBtn.onClick.AddListener(ButtonProp);
            CloseBtn.onClick.AddListener(ButtonClose);
            EquipBtnText = EquipBtn.GetComponentInChildren<Text>();
            PropBtnText = PropBtn.GetComponentInChildren<Text>();
            EquipBtnText.text = base.ShowText("Equip");
            PropBtnText.text = base.ShowText("Prop");
            EquipBtnColor = EquipBtn.image.color;
            OrangeTextColor = EquipBtnText.color;
            GreyTextColor = PropBtnText.color;
            // 第一次打开时需要从Player获取背包信息
            UpdatePack();
            // 更新装备，因为拖拽后Fit对应的ID会改变，如果每次打开时都ChangeFit的话那么后面装备上后前面的E会关掉，所以只在初始化时更新Fit
            for (int i = 0; i < player.Fit.Length; i++)
            {
                if (player.Fit[i] >= player.EquipPack.Length)
                    continue;
                if (i == (int)FitType.Medicine)
                    Fits[i].ChangeFit(PropArray[player.Fit[i]]);
                else
                    Fits[i].ChangeFit(EquipArray[player.Fit[i]]);
            }

        }

        /// <summary>
        /// 初始化子物体并获取
        /// </summary>
        private void Initialize()
        {
            if (DragImg)
                UIDragGrid.dragImg = DragImg;
            else
                Debug.LogError("没有DragImg");

            if (!ItemData)
                Debug.LogError("没有ItemData");

            //遍历获取初始化
            int i = 0;
            foreach (RectTransform r in Fit)
            {
                Fits[i] = r.GetComponent<UIFit>();
                Fits[i].uiInventory = this;
                Fits[i].FitNum = i;
                Fits[i].Initialize();
                i++;
            }
            i = 0;
            foreach (RectTransform r in EquipSlots)
            {
                EquipArray[i] = r.GetChild(0).GetComponent<UIDragGrid>();
                EquipArray[i].uiInventory = this;
                EquipArray[i].ID = i;
                EquipArray[i].IsEquip = true;
                EquipArray[i].Initialize();
                i++;
            }
            i = 0;
            foreach (RectTransform r in PropSlots)
            {
                PropArray[i] = r.GetChild(0).GetComponent<UIDragGrid>();
                PropArray[i].uiInventory = this;
                PropArray[i].ID = i;
                PropArray[i].IsEquip = false;
                PropArray[i].Initialize();
                i++;
            }
        }

        /// <summary>
        /// 每次启动时自动根据Player的信息更新背包，外部只需要修改Player的Data，不需要控制背包
        /// </summary>
        private void OnEnable()
        {
            UpdateCharacterInfo();
            // 初始显示装备界面
            ButtonEquip();
            // 进行数据初始化
            UpdatePack();
        }

        /// <summary>
        /// UI选择切换装备。Player数据更新，UI更新
        /// </summary>
        /// <param name="_grid"></param>
        public void SelectItem(UIDragGrid _grid)
        {
            if (_grid.IsEquip)
            {
                player.PlayerMedi.ChangeEquip(_grid.ID);    // Data
                Fits[(int)_grid.Equip.Type].ChangeFit(_grid);   // View
                UpdateCharacterInfo();
            }
            else if (_grid.Prop.Type == PropType.Medicine)    // 道具只能选择Medicine
            {
                player.PlayerMedi.ChangeProp(_grid.ID);
                Fits[(int)FitType.Medicine].ChangeFit(_grid);
            }
        }

        public void DropEquip(UIDragGrid _grid)
        {
            if (_grid.IsEquip)
            {
                player.PlayerMedi.DropEquip(_grid.Equip);
                UpdateCharacterInfo();
            }
            else
                player.PlayerMedi.DropProp();
        }
        /// <summary>
        /// 交换ID，提供给Grid调用
        /// </summary>
        /// <param name="_ID1"></param>
        /// <param name="_ID2"></param>
        public void ExchangeEquip(UIDragGrid _Slot1, UIDragGrid _Slot2)
        {
            int _ID1 = _Slot1.ID;
            int _ID2 = _Slot2.ID;
            IEquip _equip = player.EquipPack[_ID1];
            player.EquipPack[_ID1] = player.EquipPack[_ID2];
            player.EquipPack[_ID2] = _equip;
            // 如果该物品已装备，那么Fit对应的ID也要修改
            if (_Slot1.Selected)
                player.Fit[(int)_Slot1.Equip.Type] = _ID2;
            if (_Slot2.Selected)
                player.Fit[(int)_Slot2.Equip.Type] = _ID1;
        }   
        public void ExchangeProp(UIDragGrid _Slot1, UIDragGrid _Slot2)
        {
            int _ID1 = _Slot1.ID;
            int _ID2 = _Slot2.ID;
            IProp _Prop = player.PropPack[_ID1];
            player.PropPack[_ID1] = player.PropPack[_ID2];
            player.PropPack[_ID2] = _Prop;
            // 如果该物品已装备，那么Fit对应的ID也要修改
            if (_Slot1.Selected)
                player.Fit[(int)FitType.Medicine] = _ID2;
            if (_Slot2.Selected)
                player.Fit[(int)FitType.Medicine] = _ID1;
        }
        public void ExchangeFitEquip(UIDragGrid _grid1, UIDragGrid _grid2)
        {
            if (_grid1.Selected)
                Fits[(int)_grid1.Equip.Type].ChangeFit(_grid1);
            if (_grid2.Selected)
                Fits[(int)_grid2.Equip.Type].ChangeFit(_grid2);
        }
        public void ExchangeFitProp(UIDragGrid _grid1, UIDragGrid _grid2)
        {
            if (_grid1.Selected)
                Fits[(int)FitType.Medicine].ChangeFit(_grid1);
            if (_grid2.Selected)
                Fits[(int)FitType.Medicine].ChangeFit(_grid2);
        }

        /// <summary>
        /// 更新背包,按顺序为Grid赋值
        /// </summary>
        private void UpdatePack()
        {
            // 更新背包
            for (int i = 0; i < player.EquipPack.Length; i++)
            {
                // 不相同时才更新,null也要替换
                if (EquipArray[i].Equip != player.EquipPack[i])
                {
                    EquipArray[i].Equip = player.EquipPack[i];
                    EquipArray[i].UpdateImage();
                }
            }
            for (int i = 0; i < player.PropPack.Length; i++)
            {
                // 道具必须更新
                PropArray[i].Prop = player.PropPack[i];
                PropArray[i].UpdateImage();
            }
        }

        #region 安全方法
        /// <summary>
        /// 更新WindowCharacter的信息，均来源于player
        /// </summary>
        public void UpdateCharacterInfo()
        {
            if (player != null)
            {
                Rank.text = player.Rank.ToString();
                Spec.text = player.Special.ToString("F");
                HP.text = player.MaxHP.ToString();
                SP.text = player.MaxSP.ToString();
                ATT.text = player.AttackPoint.ToString();
                DEF.text = player.DefendPoint.ToString();
                CRI.text = player.CritPoint.ToString();
                Speed = (int)((player.MoveSpeed - 4) * 10);
                SPD.text = Speed.ToString();
                Gold.text = player.Gold.ToString();
            }
        }
        /// <summary>
        /// 更新ItemData，提供给Grid调用
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="pos"></param>
        public void ShowItemData(UIDragGrid grid, Vector3 pos)
        {
            if (ItemData != null)
            {
                IItem item;
                // 只要调用了此方法就说明一定可以获取到值，所以无需判空
                if (grid.IsEquip)
                    item = grid.Equip;
                else
                    item = grid.Prop;
                ItemName.text = item.Name;
                ItemDetail.text = string.Empty; // 先清空
                if (item.Attack > 0)
                    ItemDetail.text = "\n" + "<color=#FF8E00FF>ATT</color> +" + item.Attack;
                if (item.Defend > 0)
                    ItemDetail.text = ItemDetail.text + "\n" + "<color=#FF8E00FF>DEF</color> +" + item.Defend;
                if (item.HP > 0)
                    ItemDetail.text = ItemDetail.text + "\n" + "<color=#FF8E00FF>HP</color> +" + item.HP;
                if (item.SP > 0)
                    ItemDetail.text = ItemDetail.text + "\n" + "<color=#FF8E00FF>SP</color> +" + item.SP;
                ItemData.gameObject.SetActive(true);
                //需要设置pivot为0,1
                ItemData.transform.position = pos;
            }
        }
        /// <summary>
        /// 关闭ItemData,提供给Grid调用
        /// </summary>
        public void CloseItemData()
        {
            if (ItemData != null && ItemData.IsActive())
            {
                ItemData.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 切换显示
        /// </summary>
        private void ButtonEquip()
        {
            EquipBtn.image.color = EquipBtnColor;
            PropBtn.image.color = Color.clear;
            EquipBtnText.color = OrangeTextColor;
            PropBtnText.color = GreyTextColor;
            // 以上是Btn变化
            EquipSlots.gameObject.SetActive(true);
            PropSlots.gameObject.SetActive(false);
        }
        private void ButtonProp()
        {
            PropBtn.image.color = EquipBtnColor;
            EquipBtn.image.color = Color.clear;
            PropBtnText.color = OrangeTextColor;
            EquipBtnText.color = GreyTextColor;
            EquipSlots.gameObject.SetActive(false);
            PropSlots.gameObject.SetActive(true);
        }
        private void ButtonClose()
        {
            GameMainProgram.Instance.uiManager.CloseUIForms("Inventory");
            // Save
            player.SaveData();
        }
        #endregion
    }
}