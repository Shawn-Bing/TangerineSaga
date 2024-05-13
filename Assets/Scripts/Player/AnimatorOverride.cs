using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T_Saga.Inventory;

// 获取OverrideAnimator 控制切换玩家动画
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;

    public SpriteRenderer holdItem;//TODO:引擎中设置

    [Header("动画列表")]
    // 引擎中设置列表，使类型和控制器对应
    public List<AnimatorType> animatorTypes;
    // 创建一个字典和控制器对应
    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);//生成字典
        }
    }

    #region 注册物品选中、收获作物事件

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }

    /// <summary>
    /// 相当于选中收获的作物并举起
    /// </summary>
    /// <param name="ID"></param>
    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID)?.itemOnWorldSprite;
        if (holdItem.enabled == false)
        {
            StartCoroutine(HarvestItem(itemSprite));
        }
    }
    /// <summary>
    /// 协程，在收获作物后举起一段时间再取消举起
    /// </summary>
    /// <param name="itemSprite"></param>
    /// <returns></returns>
    private IEnumerator HarvestItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        //FIXME：在Setting中设置这个时间
        yield return new WaitForSeconds(Settings.harvestHoldTime);
        holdItem.enabled = false;
    }

    /// <summary>
    /// 在场景卸载前取消物品选中 & 举起的物品
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(HoldType.None);
    }
    #endregion

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // FIXME: 补全不同物品类型对应的动画
        // 暂时只设置种子和商品可以举起
        HoldType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => HoldType.Carry,
            ItemType.Commodity => HoldType.Carry,
            ItemType.HoeTool => HoldType.Hoe,
            ItemType.WaterTool => HoldType.Water,
            ItemType.CollectTool => HoldType.Collect,
            ItemType.ChopTool => HoldType.Chop,
            ItemType.BreakTool => HoldType.Break,
            ItemType.ReapTool => HoldType.Reap,
            ItemType.Furniture => HoldType.Carry,
            _ => HoldType.None
        };

        //若未选中，播放非Hold动画
        if (isSelected == false)
        {
            currentType = HoldType.None;
            holdItem.enabled = false;// 关闭图片显示
        }     
        else
        {
            if (currentType == HoldType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;// 设置图片为选中图
                holdItem.enabled = true;// 使图片可见
            }
            else
            {
                //切换非举起道具时取消Carry状态
                holdItem.enabled = false;
            }
        }

        SwitchAnimator(currentType);
    }

    /// <summary>
    /// 对应字典切换动画
    /// </summary>
    /// <param name="holdType"></param>
    private void SwitchAnimator(HoldType holdType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.holdType == holdType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
            else if (item.holdType == HoldType.None)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}