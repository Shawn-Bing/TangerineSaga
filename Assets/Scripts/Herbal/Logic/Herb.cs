using System.Collections;
using System.Collections.Generic;
using T_Saga.Map;
using UnityEngine;

// 收割
public class Herb : MonoBehaviour
{
    public HerbalDetails herbalDetails;
    // 存放Tile信息
    public TileDetails tileDetails;
    // 用来计算收获的点击次数
    private int harvestActionCount;
    // 获取种子身上动画组件
    private Animator anim;
    // 获取Player组件
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    public bool CanHarvest => tileDetails.growthDays >= herbalDetails.TotalGrowthDays;
    

    public void ExecuteToolAction(ItemDetails tool,TileDetails tile)
    {
        tileDetails = tile;
        anim = GetComponentInChildren<Animator>();

        //工具使用次数
        int requireActionCount = herbalDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;
        
        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            //判断是否有动画（针对树木等）
            if (anim != null && herbalDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("RotateRight");
                else
                    anim.SetTrigger("RotateLeft");
            }
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
    /// 实现收获作物，生成果实
    /// </summary>
    public void SpawnHarvestItems()
    {
        // 遍历生成物品数组，生成物品
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
        
        // 设置收割后作物参数
        // 1. 可重复收获的，重新计算生长天数，并调节作物为未成熟图片
        // 2. 不可重复生长的，清空Tile内种子信息，重置上次收获计时器
        // 3. 摧毁物体
        if(tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;
            Debug.Log("daysToRegrow = " + herbalDetails.daysToRegrow);
            Debug.Log("daysSinceLastHarvest = " + tileDetails.daysSinceLastHarvest);

            // 若有重新生长天数参数(>0) 且 总收获次数小于最大重生次数限制，说明可重复收割
            if (herbalDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < herbalDetails.maxRegrowTimes)
            {
                // 生长天数归零
                tileDetails.growthDays = herbalDetails.TotalGrowthDays - herbalDetails.daysToRegrow;
                //刷新种子
                EventHandler.CallRefreshCurrentMap();
            }
            else
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
                
                //FIXME:可设计为收获作物后将土地恢复为未耕地的样子
                // tileDetails.daysSinceDug = -1;
            }

            // 摧毁该物体
            Destroy(gameObject);
        }
    }
}
