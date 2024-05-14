using UnityEngine;
[System.Serializable]
public class HerbalDetails
{
    public int seedItemID;
    [Header("各阶段生长周期")]
    //各阶段所需天数
    public int[] growthDays;

    // 计算总天数
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("各阶段Prefab")]
    // 普通作物用itemBase
    // 树的最后阶段用树的Prefab
    public GameObject[] growthPrefabs;
    [Header("各阶段图片")]
    //设定每个阶段的item
    public Sprite[] growthSprites;
    [Header("限定种植季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("各类工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")]
    // 可重复收割的，转换为初始状态
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    
    //在最值之间随机给出果实数量
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("重复收割")]
    public int daysToRegrow;//重新生长天数
    public int maxRegrowTimes;//最大重收割次数

    [Header("特殊属性")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;
    public ParticleEffectType effectType;
    public Vector3 effectPos;


    /// <summary>
    /// 检查当前工具是否符合收获作物所用工具限制
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获得工具需要使用的次数
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
