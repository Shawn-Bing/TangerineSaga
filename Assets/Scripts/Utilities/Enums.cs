public enum ItemType
{
    #region 可买卖物品
    Seed,//种子
    Commodity,//农作物等
    Furniture,//家具
    #endregion

    #region 耕作工具
    //不可买卖
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,
    #endregion

    #region 可破坏物
    //可破坏场景物品，如杂草等
    ReapableScenery,
    #endregion
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

//人物举起状态
public enum HoldType
{
    None, Carry
}
  
//人物身体部分名称
public enum PartName
{
    Body, Hair, Arm, Tool
}