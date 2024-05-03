using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件调用集
public static class EventHandler
{
    /// <summary>
    /// 更新目标位置(背包等)的UI
    /// </summary>
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    /// <summary>
    /// 生成物品
    /// </summary>
    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
}