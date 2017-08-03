using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFramework;

namespace DreamKeeper
{
    public class UIStore:ViewBase
    {
        public Button BuyOrSaleBtn;
        public Button EquipBtn;
        public Button PropBtn;
        public Button CloseBtn;
        public RectTransform EquipSlots;
        public RectTransform PropSlots;
        public Text Gold;
        public Text Price;
        public Button BuyBtn;
        public Image ItemData;
        public Text ItemName;
        public Text ItemDetail;
        public IEquip[] EquipStore=new IEquip[35];
        public IProp[] PropStore=new IProp[35];

        private StoreData storeData;
        private bool isBuy = true;  // Buy和Sale两个界面
        private UIStoreGrid[] EquipArray = new UIStoreGrid[35];   //装备数组，存储顺序
        private UIStoreGrid[] PropArray = new UIStoreGrid[35];    //道具数组
        private int lockID=99;
        private IPlayer player;
        private Text BuyOrSaleBtnText;
        private Text EquipBtnText;
        private Text PropBtnText;
        private Text BuyBtnText;
        private Color EquipBtnColor;
        private Color OrangeTextColor;
        private Color GreyTextColor;

        private void Awake()
        {
            base.UIForm_Type = UIFormType.PopUp;
            base.UIForm_ShowMode = UIFormShowMode.ReverseChange;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer;

            // 注册事件
            BuyBtn.onClick.AddListener(ButtonBuy);
            BuyOrSaleBtn.onClick.AddListener(ButtonBuyOrSale);
            EquipBtn.onClick.AddListener(ButtonEquip);
            PropBtn.onClick.AddListener(ButtonProp);
            CloseBtn.onClick.AddListener(ButtonClose);
            // 语言国际化
            BuyOrSaleBtnText = BuyOrSaleBtn.GetComponentInChildren<Text>();
            EquipBtnText = EquipBtn.GetComponentInChildren<Text>();
            PropBtnText = PropBtn.GetComponentInChildren<Text>();
            BuyBtnText = BuyBtn.GetComponentInChildren<Text>();
            //BuyOrSaleBtnText.text = base.ShowText("Buy");
            EquipBtnText.text = base.ShowText("Equip");
            PropBtnText.text = base.ShowText("Prop");
            //BuyBtnText.text = base.ShowText("Buy");
            EquipBtnColor = EquipBtn.image.color;
            OrangeTextColor = EquipBtnText.color;
            GreyTextColor = PropBtnText.color;

            if (!ItemData)
                Debug.LogError("没有ItemData");
            int i = 0;
            foreach (RectTransform r in EquipSlots)
            {
                EquipArray[i] = r.GetComponentInChildren<UIStoreGrid>();
                EquipArray[i].uiStore = this;
                EquipArray[i].ID = i;
                EquipArray[i].IsEquip = true;
                EquipArray[i].Initialize();
                i++;
            }
            i = 0;
            foreach (RectTransform r in PropSlots)
            {
                PropArray[i] = r.GetComponentInChildren<UIStoreGrid>();
                PropArray[i].uiStore = this;
                PropArray[i].ID = i;
                PropArray[i].IsEquip = false;
                PropArray[i].Initialize();
                i++;
            }
            // Store更新必定在初始化时
            LoadStoreData();

        }

        private void OnEnable()
        {
            Gold.text = player.Gold.ToString();
            // 初始显示购买界面
            isBuy = false;
            ButtonBuyOrSale();
            // 初始显示装备界面
            ButtonEquip();
        }

        private void LoadStoreData()
        {
            storeData=GameMainProgram.Instance.fileMgr.LoadJsonSaveData<StoreData>(Application.dataPath+"/Store");
            if (storeData == default(StoreData))
            {
                storeData = new StoreData();
                GameMainProgram.Instance.fileMgr.CreateJsonSaveData(Application.dataPath + "/Store", storeData);
                Debug.Log("商店已存档于"+ Application.dataPath);
            }


            // 获取数据
            for(int i=0;i<storeData.EquipKeys.Length;i++)
            {
                if (storeData.EquipKeys[i] == null)
                    EquipStore[i] = null;
                else
                {
                    switch(storeData.EquipKeys[i].Type)
                    {
                        case FitType.Weapon:
                            EquipStore[i] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicWeapon, storeData.EquipKeys[i].Key);
                            break;
                        case FitType.Cloth:
                            EquipStore[i] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicCloth, storeData.EquipKeys[i].Key);
                            break;
                        case FitType.Shoe:
                            EquipStore[i] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicShoe, storeData.EquipKeys[i].Key);
                            break;
                        default:
                            break;
                    }
                }
            }

            for (int i = 0; i < storeData.PropKeys.Length; i++)
            {
                if (storeData.PropKeys[i] == null)
                    PropStore[i] = null;
                else
                {
                    switch (storeData.PropKeys[i].Type)
                    {
                        case PropType.Medicine:
                            PropStore[i] = UnityHelper.FindDic(GameMainProgram.Instance.dataBaseMgr.dicMedicine, storeData.PropKeys[i].Key);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void UpdateStore()
        {
            // UI更新，null也要替换
            for (int i = 0; i < EquipStore.Length; i++)
            {
                EquipArray[i].Equip = EquipStore[i];
                EquipArray[i].UpdateBuy();
            }
            for (int i = 0; i < PropStore.Length; i++)
            {
                PropArray[i].Prop = PropStore[i];
                PropArray[i].UpdateBuy();
            }
        }

        /// <summary>
        /// 获取Player背包信息
        /// </summary>
        private void UpdateStoreSale()
        {
            // 更新显示为Player背包,null也要替换
            for (int i = 0; i < player.EquipPack.Length; i++)
            {
                EquipArray[i].Equip = player.EquipPack[i];
                EquipArray[i].UpdateSale();
            }
            for (int i = 0; i < player.PropPack.Length; i++)
            {
                PropArray[i].Prop = player.PropPack[i];
                PropArray[i].UpdateSale();
            }
            // 显示装备了的物体
            for(int i=0;i<player.Fit.Length;i++)
            {
                if (player.Fit[i] < player.EquipPack.Length)
                {
                    if(i==(int)FitType.Medicine)
                        PropArray[player.Fit[i]].E.gameObject.SetActive(true);
                    else
                        EquipArray[player.Fit[i]].E.gameObject.SetActive(true);
                }
            }
        }

        public void LockEquip(int _ID)
        {
            if (lockID < EquipStore.Length) //将当前锁定的解锁
                EquipArray[lockID].UnLock();
            lockID = _ID;
            // 购买和出售显示略有不同
            if(isBuy)
                Price.text = "-"+EquipStore[lockID].Price;
            else
                Price.text = "+"+ EquipArray[lockID].Equip.SalePrice;
            Price.transform.parent.gameObject.SetActive(true);
        }

        /// <summary>
        /// 切换Btn，点击其他都要UnLock
        /// </summary>
        /// <param name="_ID"></param>
        public void UnLock()
        {
            // 不管是哪个界面，统统解锁
            if (lockID < EquipStore.Length) //将当前锁定的解锁
                EquipArray[lockID].UnLock();
            if (lockID < PropStore.Length) //将当前锁定的解锁
                PropArray[lockID].UnLock();
            lockID = 99;
            Price.transform.parent.gameObject.SetActive(false);
        }

        public void LockProp(int _ID)
        {
            if (lockID < PropStore.Length) //将当前锁定的解锁
                PropArray[lockID].UnLock();
            lockID = _ID;
            if (isBuy)
                Price.text = "-" + PropStore[lockID].Price;
            else
                Price.text = "+" + PropArray[lockID].Prop.SalePrice;
            Price.transform.parent.gameObject.SetActive(true);
        }

        /// <summary>
        /// 更新ItemData，提供给Grid调用
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="pos"></param>
        public void ShowItemData(UIStoreGrid grid, Vector3 pos)
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
        /// 购买成功增加道具，购买失败显示提示
        /// </summary>
        private void ButtonBuy()
        {
            // 未锁定
            if (lockID > EquipStore.Length)
                return;
            // 装备
            if(EquipSlots.gameObject.activeSelf)
            {
                // 购买
                if (isBuy)
                {
                    if (EquipStore[lockID].Price > player.Gold)
                    {
                        UIPromptBox.wordID = 4;
                        GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                        return;
                    }
                    if (player.AddItem(EquipStore[lockID]))
                        player.Gold -= EquipStore[lockID].Price;
                    // 失败的话说明背包已满
                    else
                    {
                        UIPromptBox.wordID = 3;
                        GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    }
                }
                // 出售
                else
                {
                    // View层移除后数据层也要移除
                    if (EquipArray[lockID].SaleItem())
                    {
                        // 先拿钱再交货
                        player.Gold += player.EquipPack[lockID].SalePrice;
                        player.RemoveEquip(lockID); // 让背包中对应的装备移除
                    }
                        UnLock();   // 无论装备是否可以卖都要解锁
                }
            }
            else
            {
                // 购买道具
                if (isBuy)
                {
                    if (PropStore[lockID].Price > player.Gold)
                    {
                        UIPromptBox.wordID = 4;
                        GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                        return;
                    }
                    if (player.AddItem(PropStore[lockID]))
                        player.Gold -= PropStore[lockID].Price;
                    // 失败的话说明已满
                    else
                    {
                        UIPromptBox.wordID = 2;
                        GameMainProgram.Instance.uiManager.ShowUIForms("PromptBox");
                    }
                }
                // 出售
                else
                {
                    if (PropArray[lockID].SaleItem())
                    {
                        player.Gold += player.PropPack[lockID].SalePrice;
                        player.RemoveProp(lockID);   // 因为UI和Player引用的是同一个Prop，所以Num是一样的，但是UI中置空后背包也应该置空
                        if(PropArray[lockID].Prop==null)
                            UnLock();   // 出售完了的话解锁
                    }
                    else
                        UnLock();   // 不可出售的话解锁

                }
            }
            Gold.text = player.Gold.ToString();
        }
        private void ButtonBuyOrSale()
        {
            if (isBuy)
            {
                BuyOrSaleBtnText.text = base.ShowText("Sale");
                BuyBtnText.text = base.ShowText("Sale");
                isBuy = false;
                UpdateStoreSale();
            }
            else
            {
                BuyOrSaleBtnText.text = base.ShowText("Buy");
                BuyBtnText.text = base.ShowText("Buy");
                isBuy = true;
                UpdateStore();
            }
            UnLock();
        }
        private void ButtonEquip()
        {
            EquipBtn.image.color = EquipBtnColor;
            PropBtn.image.color = Color.clear;
            EquipBtnText.color = OrangeTextColor;
            PropBtnText.color = GreyTextColor;
            // 以上是Btn变化
            EquipSlots.gameObject.SetActive(true);
            PropSlots.gameObject.SetActive(false);
            // Unlock
            UnLock();
        }
        private void ButtonProp()
        {
            PropBtn.image.color = EquipBtnColor;
            EquipBtn.image.color = Color.clear;
            PropBtnText.color = OrangeTextColor;
            EquipBtnText.color = GreyTextColor;
            EquipSlots.gameObject.SetActive(false);
            PropSlots.gameObject.SetActive(true);

            UnLock();
        }
        private void ButtonClose()
        {
            UnLock();
            GameMainProgram.Instance.uiManager.CloseUIForms("Store");
            // 存档
            player.SaveData();
        }
    }
}