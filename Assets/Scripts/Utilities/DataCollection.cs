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
//每个结构体都要序列化
[System.Serializable]
//创建结构体不需要判空，默认会给值
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
// 存放玩家动画类型,是空闲/举起 + 身体部位的排列组合，这个排列组合应当对应动画控制器
public class AnimatorType
{
    public HoldType holdType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
// 存放玩家的坐标
public class SerializableVector3
{
    public float x, y, z;
    //序列化为一个向量
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    //返回位置向量
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    // 返回基于瓦片地图的2D坐标
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
// 存放场景中的物品
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}