using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using T_Saga.Dialogue;

// 事件调用集
public static class EventHandler
{

    #region 背包
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
    /// 扔出物品(从背包)
    /// </summary>
    public static event Action<int, Vector3, ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }
    #endregion

    #region 时间
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
    /// 每天更新一次，用于更新种植
    /// </summary>
    public static event Action<int,Season> GameDayEvent;
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    #endregion

    #region 种植
    /// <summary>
    /// 我种下一颗种子
    /// </summary>
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }

    /// <summary>
    /// 在玩家坐标处生成物品(作物)
    /// </summary>
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPositionEvent(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    /// <summary>
    /// 刷新地图
    /// 用于刷新可重复收割作物
    /// </summary>
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    /// <summary>
    /// 播放粒子效果（砍树时）
    /// </summary>
    public static event Action<ParticleEffectType, Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateHerbEvent;
    public static void CallGenerateHerbEvent()
    {
        GenerateHerbEvent?.Invoke();
    }
    #endregion

    #region 场景切换
    /// <summary>
    /// 带有位置移动的切换场景事件
    /// </summary>
    public static event Action<string,Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 position)
    {
        TransitionEvent?.Invoke(sceneName, position);
    }
    #endregion

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

    /// <summary>
    /// 鼠标点击事件(传递选中了物品的信息)
    /// </summary>
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    /// <summary>
    /// 在播放动画后，执行各类方法如砍树、播种等
    /// </summary>
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    #region 对话事件
    /// <summary>
    /// 显示对话
    /// </summary>
    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece piece)
    {
        ShowDialogueEvent?.Invoke(piece);
    }

    //商店开启
    public static event Action<SlotType, InventoryRepo_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryRepo_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }

    public static event Action<SlotType, InventoryRepo_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryRepo_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }

    
    public static event Action<ItemDetails, bool> ShowTradeUI;
    /// <summary>
    /// 显示交易面板
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="isSell">买</param>
    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }

    #endregion

    /// <summary>
    /// 游戏暂停事件
    /// </summary>
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }
}