using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 为背包系统创建单独命名空间，防止乱调用、耦合
namespace T_Saga.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemDataList_SO;

        // 通过ID查找物品信息
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

    }
}
