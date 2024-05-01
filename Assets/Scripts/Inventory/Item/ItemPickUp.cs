using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    //人物与物体发生碰撞时拾取物品
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();

            if (item != null)
            {
                if (item.itemDetails.canPickedUp)
                {
                    //添加到背包
                    InventoryManager.Instance.AddItem(item, true);
                }
            }
        }
    }
}
