public enum ItemType
{
Seed,Commodity,Furniture,//种子、商品、家具
HoeTool,ChopTool,BreakTool,ReapToop,WaterTool,CollectTool,//各种工具
ReapableScenery//可破坏场景物品，如杂草
}

//物品格子类型
public enum SlotType
{
    PlayerBag,//玩家背包
    Box,//箱子
    Shop//商店
}

//UI更新位置
public enum InventoryLocation
{
    PlayerBag,Box,
}