using System;
using UnityEngine;
using UnityEngine.UI;

namespace T_Saga.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public Text itemName;
        public InputField tradeAmount;
        public Button submitButton;
        public Button cancelButton;
        private ItemDetails item;
        private bool isSellTrade;

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 设置TradeUI详情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetUpTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        /// <summary>
        /// 执行交易
        /// </summary>
        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);
            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);
            CancelTrade();
        }

        /// <summary>
        /// 关闭交易窗口
        /// </summary>
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}
