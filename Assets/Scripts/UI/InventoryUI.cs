using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace T_Saga.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;
        
        [Header("背包格子")]
        [SerializeField] private SlotUI[] playerSlots;


        #region 注册调用事件
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
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
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)//只有数量>0才更新
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }
        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                // 获取playerBag里的Slot序号并连接到UI部分
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;//获取背包状态(是否开启)
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
        /// 切换打开&关闭背包
        /// </summary>
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }
    }
}
