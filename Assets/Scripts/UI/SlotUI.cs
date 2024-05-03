using UnityEngine;
using UnityEngine.UI;
using TMPro;//TextMeshPro
using UnityEngine.EventSystems;
using T_Saga.Inventory;//Click事件

namespace T_Saga
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler
    {
        // 不在Awake中获取组件，而是在Inspector中获取(更快)
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI moneyAmountText;
        [SerializeField] public Image slotHighlight;//代码高亮
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();//调节代码高亮所需
        [SerializeField] private Button button;

        [Header("格子参数")]
        public SlotType slotType;
        public bool isSelected;//格子选中状态
        public int slotIndex;//格子序号

        [Header("物品详情&数量")]
        public ItemDetails itemDetails;
        public int itemAmount;

        //初始化，若格子中没有任何物体就清空全部格子
        private void Start()
        {
            isSelected = false;
            // public变量会初始化，不能用itemDetails == null来判断
            if (itemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }


        /// <summary>
        /// 更新格子
        /// </summary>
        /// <param name="item">更新物品</param>
        /// <param name="amount">更新数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            // 保存数据
            itemDetails = item;
            itemAmount = amount;

            // 更新格子UI，基本是逆清空操作
            slotImage.enabled = true;
            slotImage.sprite = item.itemIcon;
            moneyAmountText.text = amount.ToString();
            button.interactable = true;
        }

        /// <summary>
        /// 清空格子
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }
            
            itemAmount = 0;//设置数量为0
            slotImage.enabled = false;//关闭图片显示
            moneyAmountText.text = string.Empty;//清空文字
            button.interactable = false;//取消按键可交互
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            isSelected = !isSelected; // 这里只切换选中状态，使选中唯一在InventoryUI中实现
            inventoryUI.UpdateSlotHighlight(slotIndex);
        }
    }
}
