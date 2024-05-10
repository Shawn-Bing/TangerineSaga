using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T_Saga.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemTips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //获取父级组件（ItemTip是在Slot旁边显示的，SlotUI又挂在InventoryUI上）
        private SlotUI slotUI;
        private InventoryUI InvtrUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        /// <summary>
        /// 指针移入时，显示tips
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 当物品详情不为空时才显示(丢弃物品)
            if (slotUI.itemDetails != null)
            {
                InvtrUI.itemTips.gameObject.SetActive(true);
                InvtrUI.itemTips.SetupItemTip(slotUI.itemDetails, slotUI.slotType);
                //修改提示框锚点&位置，4K Resolution下，向上移动60较好
                InvtrUI.itemTips.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                InvtrUI.itemTips.transform.position = transform.position + Vector3.up * 60;
            }
            // 空格子也关闭显示
            else
            {
                InvtrUI.itemTips.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 指针移出时，关闭tips
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            InvtrUI.itemTips.gameObject.SetActive(false);
        }
    }
}