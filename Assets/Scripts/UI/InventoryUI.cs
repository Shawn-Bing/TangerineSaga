using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace T_Saga.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        #region 获取Object
        //TODO 引擎中获取
        [Header("物品提示")]
        public ItemTips itemTips;

        [Header("拖拽图片")]
        //TODO:记得在InventoryUI中给它赋值
        //拖拽图Image属性不要勾选RayCast Target否则拖拽过程中遇到物体就会中断拖拽
        public Image dragItem;
        
        [Header("玩家背包UI")]
        [SerializeField] private GameObject playerBagUI;
        private bool bagOpened;
        
        [Header("其他背包")]
        [SerializeField] private GameObject baseBagUI;
        public GameObject shopSlotPrefab;
        public GameObject boxSlotPrefab;

        [Header("交易界面")]
        public TradeUI tradeUI;


        [Header("背包格子")]
        // 最好固定为26
        [SerializeField] private SlotUI[] playerBagSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;
        #endregion
        

        #region 注册UI事件
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }

        /// <summary>
        /// 关闭所有高亮
        /// </summary>
        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHighlight(-1);
        }

        /// <summary>
        /// 打开商店或箱子事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryRepo_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null,
            };

            //生成背包UI
            baseBagUI.SetActive(true);

            baseBagSlots = new List<SlotUI>();

            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBagUI.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //强制刷新
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBagUI.GetComponent<RectTransform>());

            if (slotType == SlotType.Shop)
            {
                playerBagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                playerBagUI.SetActive(true);
                bagOpened = true;
            }
            //更新UI显示
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
        }

        /// <summary>
        /// 关闭商店或箱子
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryRepo_SO bagData)
        {
            baseBagUI.SetActive(false);
            itemTips.gameObject.SetActive(false);
            UpdateSlotHighlight(-1);

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotType.Shop)
            {
                playerBagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                playerBagUI.SetActive(false);
                bagOpened = false;
            }
        }


        /// <summary>
        /// 显示交易界面
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetUpTradeUI(item, isSell);
        }

        #endregion

        /// <summary>
        /// 更新背包UI显示
        /// </summary>
        /// <param name="location">UI位置</param>
        /// <param name="list">背包等位置的物品列表</param>
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.PlayerBag:
                    for (int i = 0; i < playerBagSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)//只有数量>0才更新
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                case InventoryLocation.Shop:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            // TODO：更新玩家金钱
            //playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        private void Start()
        {
            for (int i = 0; i < playerBagSlots.Length; i++)
            {
                // 获取playerBag里的Slot序号并连接到UI部分
                playerBagSlots[i].slotIndex = i;
            }
            bagOpened = playerBagUI.activeInHierarchy;//获取背包状态(是否开启)
        }

        /// <summary>
        /// 检测按下B键打开背包
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        /// <summary>
        /// 切换打开&关闭背包UI
        /// </summary>
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            playerBagUI.SetActive(bagOpened);
        }

        /// <summary>
        /// 更新高亮并使高亮唯一
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in playerBagSlots)
            {
                // 若格子被选中 且 格子编号=传入编号
                if(slot.isSelected && slot.slotIndex == index)
                {
                    // 将格子设为高亮
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected= false;// 取消其他格子选中状态
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
    }
}
