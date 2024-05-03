using UnityEngine;
using UnityEngine.UI;
using TMPro;//TextMeshPro
using UnityEngine.EventSystems;
using T_Saga.Inventory;//Click事件

namespace T_Saga
{
    //SlotUI继承的都是Unity已写好的事件
    public class SlotUI : MonoBehaviour,
    IPointerClickHandler,   //点击事件
    IBeginDragHandler,      //开始拖拽事件
    IDragHandler,           //拖拽过程事件
    IEndDragHandler         //结束拖拽事件
    {
        // 不在Awake中获取组件，而是在Inspector中获取(更快)
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI moneyAmountText;
        public Image slotHighlight;//存放代码高亮图片
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
        
        #region 调用事件接口
        /// <summary>
        /// (只实现)点击切换选中状态，使选中唯一在InventoryUI中实现
        /// </summary>

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            isSelected = !isSelected;
            inventoryUI.UpdateSlotHighlight(slotIndex);
        }

        /// <summary>
        /// 开始拖拽时生成带有图标的临时物体，并对被拖拽物品设置高亮
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true; // 将拖拽图设为可见
                inventoryUI.dragItem.sprite = slotImage.sprite; // 给拖拽图赋值
                inventoryUI.dragItem.SetNativeSize(); //设置为图片原来尺寸

                isSelected = true;// 设为高亮
                inventoryUI.UpdateSlotHighlight(slotIndex); 
            }
        }

        /// <summary>
        /// 拖拽过程中图标跟随指针
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition; //拖拽的图片等于鼠标的位置
        }

        /// <summary>
        /// 停止拖拽时
        /// 1. 清空临时图片，将其设为不可见。
        /// 2. 将物品由原Slot移动到指针位置处的Slot，交换两个Slot序号
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled=false;
            //输出指针碰到的object，为了能正确检测到slot，最好将Slot子物体的 RayCast Target都关闭
            Debug.Log(eventData.pointerCurrentRaycast.gameObject);
        }

        #endregion
    }
}
