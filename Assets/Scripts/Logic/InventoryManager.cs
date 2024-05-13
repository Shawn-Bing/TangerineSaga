using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 为背包系统创建单独命名空间，防止乱调用、耦合
namespace T_Saga.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        //TODO： 以下SO类型记得在Inventory Manger (object) 中赋值否则会报空
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryRepo_SO playerBag;


        #region 注册扔出物品、收获作物事件
        private void OnEnable() {
            EventHandler.DropItemEvent += OnDropItem;
            EventHandler.HarvestAtPlayerPosition += OnCallHarvestAtPlayerPosition;
        }

        private void OnDisable() {
            EventHandler.DropItemEvent -= OnDropItem;
            EventHandler.HarvestAtPlayerPosition -= OnCallHarvestAtPlayerPosition;
        }

        private void OnDropItem(int ID, Vector3 pos,ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        private void OnCallHarvestAtPlayerPosition(int ID)
        {
            // 直接向背包中添加物品
            var index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);

            // 更新背包UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.PlayerBag, playerBag.itemList);
        }
        #endregion


        /// <summary>
        /// 在游戏开始时调用一次更新UI，应对读档情况
        /// </summary>
        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.PlayerBag, playerBag.itemList);
        }


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
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);//添加物品
            //测试代码
            Debug.Log("你捡起了ID为"+GetItemDetails(item.itemID).itemID + "的" + GetItemDetails(item.itemID).itemName);
            if (toDestroy)
            {
                Destroy(item.gameObject);//添加到背包后，摧毁这个（在地面的）物品
            }
            //调用函数，更新背包或其他UI（实际实现不在这里）
            EventHandler.CallUpdateInventoryUI(InventoryLocation.PlayerBag, playerBag.itemList);
        }

        /// <summary>
        /// 检查背包是否有空位并返回序号,若数量为0说明有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemAmount == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>若返回-1说明背包没有这个物品，若已存在则返回物品ID</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 在背包指定位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">背包位置</param>
        /// <param name="amount">物品数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            // 背包没有这个物品且背包有容量
            if (index == -1 && CheckBagCapacity())
            {
                var item = new InventoryItem
                {
                    itemID = ID,
                    itemAmount = amount
                };
                for(int i = 0; i < playerBag.itemList.Count;i++)
                {
                    if(playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }

            // 背包内有这个物品，只增加Amount
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// 移除N个玩家背包内物品
        /// </summary>
        /// <param name="ID">要移除的物品ID</param>
        /// <param name="RemoveAmount">移除数量</param>
        public void RemoveItem(int ID,int RemoveAmount)
        {
            var index = GetItemIndexInBag(ID);//获取物品背包内ID

            // 检测背包内物品数量是否大于移除数量，若是，更新背包内物品数据
            if (playerBag.itemList[index].itemAmount > RemoveAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - RemoveAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            // 若恰好等于，清空该ID物品
            else if (playerBag.itemList[index].itemAmount == RemoveAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }

            // 更新玩家背包UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.PlayerBag, playerBag.itemList);
        }

        /// <summary>
        /// 交换物品序号以实现拖拽物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="toIndex">目标序号</param>
        public void SwapItem(int fromIndex, int toIndex)
        {

            InventoryItem currentItem = playerBag.itemList[fromIndex]; // 当前物品
            InventoryItem targetItem = playerBag.itemList[toIndex]; // 目标物品

            if(targetItem.itemID != 0)// 目标物品不为空
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[toIndex] = currentItem;
            }else
            {
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            //  更新背包
            EventHandler.CallUpdateInventoryUI(InventoryLocation.PlayerBag, playerBag.itemList);
        }
    
    }
}
