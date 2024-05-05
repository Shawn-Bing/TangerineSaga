using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 获取OverrideAnimator 控制切换玩家动画
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;

    public SpriteRenderer holdItem;

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

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // TODO:不同的物品类型返回不同的动画，其余类型在此处补全
        // 暂时只设置种子和商品可以举起
        HoldType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => HoldType.Carry,
            ItemType.Commodity => HoldType.Carry,
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
            // else
            // {
            //     holdItem.enabled = false;
            // }
        }

        SwitchAnimator(currentType);
    }

    /// <summary>
    /// 对应字典切换动画
    /// </summary>
    /// <param name="holdType"></param>
    private void SwitchAnimator(HoldType holdType)
    {
        foreach (var anim in animatorTypes)
        {
            if (anim.holdType == holdType)
            {
                animatorNameDict[anim.partName.ToString()].runtimeAnimatorController = anim.overrideController;
            }
        }
    }
}