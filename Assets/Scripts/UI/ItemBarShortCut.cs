using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ItemBarShortCut : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;
        private bool canUse;

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }
        private void OnEnable() {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }
        private void OnDisable() {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }
        private void OnUpdateGameStateEvent(GameState gameState)
        {
            // 若暂停游戏，禁止玩家使用物品快捷键
            canUse = gameState == GameState.Gameplay;
        }

        private void Update()
        {
            if (Input.GetKeyDown(key) && canUse)
            {
                // 若当前格子非空
                if (slotUI.itemDetails != null)
                {
                    // 切换选中状态
                    slotUI.isSelected = !slotUI.isSelected;
                    if (slotUI.isSelected)
                        slotUI.inventoryUI.UpdateSlotHighlight(slotUI.slotIndex);
                    else
                        slotUI.inventoryUI.UpdateSlotHighlight(-1);

                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
                }
            }
        }
    }
}