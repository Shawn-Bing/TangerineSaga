using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 为背包系统创建单独命名空间，防止乱调用、耦合
namespace T_Saga.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        // 以下SO类型记得在Inventory Manger (object) 中赋值否则会报空
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryRepo_SO playerBag;

        // 通过ID查找物品信息
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        // 添加物品到背包

        /// <summary>
        /// 向背包中添加物品需要先进行判断：
        /// 1. 背包内是否已经有了该物体，若有，只增加数量，若无就增加ID和数量
        /// 2. 背包是否还有空位，若有则可以添加，若已满就不能捡起
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestroy">摧毁物体(可选)</param>
        public void AddItem(Item item, bool toDestroy)
        { 
            //新建一个物体且令其数量为1
            InventoryItem newItem = new InventoryItem();
            newItem.itemID = item.itemID;
            newItem.itemAmount =  1;
            //将物体添加到玩家背包第一个物品中
            playerBag.itemList[0] = newItem;
            //测试代码
            Debug.Log("你捡起了ID为"+GetItemDetails(item.itemID).itemID + "的" + GetItemDetails(item.itemID).itemName);
            if (toDestroy)
            {
                Destroy(item.gameObject);//添加到背包后，摧毁这个（在地面的）物品
            }
        }
    }
}
