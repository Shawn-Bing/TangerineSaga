using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTips : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textType;
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private Text textValue;//Text(Legacy)
    [SerializeField] private GameObject bottomPart;//Control Coin icon & Value

    /// <summary>
    /// 将物品详情信息赋值给ItemTips的UI
    /// </summary>
    /// <param name="itemDetails">物品编辑器里填写的各项数据</param>
    /// <param name="slotType">获取格子类型，在商店中显示购买价格，在背包中只显示售价(购买价格 * 售出比例)</param>
    public void SetupItemTip(ItemDetails itemDetails, SlotType slotType)
    {
        textName.text = itemDetails.itemName;
        textType.text = GetItemType(itemDetails.itemType);
        //textType.text = itemDetails.itemType.ToString();
        //GetItemType(itemDetails.itemType);
        textDescription.text = itemDetails.itemDescription;
        
        // 只有可买卖物品才显示底部售价等
        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            var price = itemDetails.itemPrice;
            if (slotType == SlotType.PlayerBag)
            {
                price = (int)(price * itemDetails.sellDiscount);
            }

            textValue.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }
        
        //查看不同物品的Tips时，令Unity立即刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    /// <summary>
    /// 修改物品名称为中文
    /// </summary>
    /// <param name="itemType">英文物品类型名称</param>
    /// <returns>中文物品类型名称</returns>
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed =>        "种子",
            ItemType.Commodity =>   "商品",
            ItemType.Furniture =>   "家具",
            ItemType.HoeTool =>     "工具",
            ItemType.ChopTool =>    "工具",
            ItemType.BreakTool =>   "工具",
            ItemType.ReapTool =>    "工具",
            ItemType.WaterTool =>   "工具",
            ItemType.CollectTool => "工具",
            _ => "希腊奶"//默认类型
        };
    }
}
