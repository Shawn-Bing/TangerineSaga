using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 收割
public class Herb : MonoBehaviour
{
    public HerbalDetails herbalDetails;
    public TileDetails tileDetails;
    // 用来计算收获的点击次数
    private int harvestActionCount;
    public bool CanHarvest => tileDetails.growthDays >= herbalDetails.TotalGrowthDays;

    private Animator anim;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    
    

    public void ExecuteToolAction(ItemDetails tool)
    {
        //工具使用次数
        int requireActionCount = herbalDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            //TODO:判断是否有动画（针对树木等）
            //TODO:播放粒子
            //TODO:播放声音
        }

        if (harvestActionCount >= requireActionCount)
        {
            // 若点击次数够了，在人物头顶部位生成
            if (herbalDetails.generateAtPlayerPosition || !herbalDetails.hasAnimation)
            {
                //生成农作物
                SpawnHarvestItems();
            }
        }
    }

    /// <summary>
    /// 生成果实
    /// </summary>
    public void SpawnHarvestItems()
    {
        // 遍历生成物品数组
        for (int i = 0; i < herbalDetails.producedItemID.Length; i++)
        {
            // 创建生成数量的临时变量
            int spawnAmount;

            // 对生成数量做区分
            // 1.无最值区间，只生成指定数量，随便赋给最值即可
            // 2.有区间，用Random生成区间内随机数
            if (herbalDetails.producedMinAmount[i] == herbalDetails.producedMaxAmount[i])
            {
                spawnAmount = herbalDetails.producedMinAmount[i];
            }
            else
            {
                spawnAmount = Random.Range(herbalDetails.producedMinAmount[i], herbalDetails.producedMaxAmount[i] + 1);
            }

            // 依据生成数量，生成物品
            for (int j = 0; j < spawnAmount; j++)
            {
                // 两种情况
                // 1. 若在田地内收获作物，则在人物头顶生成
                // 2. 若为场景内可破坏物品，在物品旁边生成
                if (herbalDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPositionEvent(herbalDetails.producedItemID[i]);
                }
                //TODO:在物品旁边生成
            }
        }
    }
}
