using System;
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

        // 添加物品到背包
        public void AddItem(Item item, bool toDestroy)
        {
            //测试代码
            Debug.Log("你捡起了ID为"+GetItemDetails(item.itemID).itemID + "的" + GetItemDetails(item.itemID).itemName);
            if (toDestroy)
            {
                Destroy(item.gameObject);//添加到背包后，摧毁这个（在地面的）物品
            }
        }
    }
}
