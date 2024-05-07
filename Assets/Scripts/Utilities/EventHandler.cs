using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    /// <summary>
    /// 选中物品
    /// </summary>
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    /// <summary>
    /// 由于小时、分钟较快，单独设立事件通知其他物体更新状态
    /// </summary>
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }

    /// <summary>
    /// 更新Hour会造成Hour上级时间单位改变
    /// </summary>
    public static event Action<int, int, int, int, Season> GameHourEvent;
    public static void CallGameHourEvent(int hour, int day, int month, int year, Season season)
    {
        GameHourEvent?.Invoke(hour, day, month, year, season);
    }

    /// <summary>
    /// 带有位置移动的切换场景事件
    /// </summary>
    public static event Action<string,Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 position)
    {
        TransitionEvent?.Invoke(sceneName, position);
    }

    /// <summary>
    /// 调整方法优先级事件
    /// </summary>
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    /// <summary>
    /// 移动坐标事件
    /// </summary>
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }
}