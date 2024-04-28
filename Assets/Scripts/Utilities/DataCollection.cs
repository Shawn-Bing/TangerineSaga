using UnityEngine;
[System.Serializable]//让Unity能识别用户写的结构体
public class ItemDetails
{
    [Header("物品详情")]
    public int itemID;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public ItemType itemType;
    
    //世界坐标下的图标不会有描边
    public Sprite itemOnWorldSprite;
    [Header("使用方式")]
    public bool canPickedUp;
    public bool canDropped;
    public bool canCarried;
    [Header("物品属性")]
    public int itemUseRadius;
    public int itemPrice;

    [Range(0,1)]//限制下方变量范围是0~1
    public float sellDiscount;


}