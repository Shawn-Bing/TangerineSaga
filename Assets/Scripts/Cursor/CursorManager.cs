using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using T_Saga.Map;
using System;

public class CursorManager : MonoBehaviour
{
    [Header("指针图像")]
    public Sprite normal, tool, seed, item, invalid;// 图标类型
    private Sprite currentSprite;   // 存储当前鼠标图片
    private Image cursorImage;      // 获取鼠标图片
    private RectTransform cursorCanvas;// 获取UI类型
    
    [Header("区域检测")]
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;//场景加载完毕之前禁用Cursor
    private bool cursorPositionValid;
    private ItemDetails currentItem;// 存放当前瓦片信息等
    
    //获取人物组件
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    
    #region 注册物品选中、鼠标可用状态、切换物品事件
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
    }

    // 物品选中事件
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if(!isSelected)
        {
            currentItem = null;
            cursorEnable = false;//没选中任何物体时禁用Cursor
            currentSprite = normal;//取消选择切换回Normal Cursor
        }
        else
        {
            //选中物体时传递Tile信息
            currentItem = itemDetails;

            //FIXME: 添加所有类型对应Cursor图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal,
            };

            //启用
            cursorEnable = true;
        }
    }

    //当选中工具时，监测玩家输入
    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    // 加载场景前
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;//关闭鼠标防止丢失坐标信息报错
    }
    
    // 加载场景后要做的事件
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();//获得Grid
    }

    #endregion


    #region 设置Cursor图片、颜色

    /// <summary>
    /// 设置Cursor图片
    /// 初始化颜色
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    // 可用状态Cursor
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1,1,1,1);
    }

    // 不可用状态Cursor
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        SetCursorImage(invalid);
        cursorImage.color = new Color(1,0,0,0.4f);
    }

    #endregion


    #region 监测鼠标可用状态
    /// <summary>
    /// 监测鼠标是否可用
    /// 判断依据是Map Data中的GridType
    /// 返回网格的世界坐标&网格坐标
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        //这里减去了Main Camera和屏幕的距离
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        

        //获取人物网格坐标。实现UseRadius
        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        

        // 若不在使用范围内，设为禁用Cursor
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }
        // 测试输出鼠标所指世界坐标
        // Debug.Log("WorldPos:" + mouseWorldPos + "\n" + "GridPos:" + mouseGridPos);

        // 获取指针处tile信息
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)//若当前选中的瓦片有信息（如canDig、CanDrop)
        {
            //FIXME: 补全切换鼠标可用情况
            // 切换Tile信息
            switch(currentItem.itemType)
            {
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped){SetCursorValid();}
                    else{SetCursorInValid();}
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig){SetCursorValid();} 
                    else {SetCursorInValid();}
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) {SetCursorValid();}
                    else {SetCursorInValid();}
                    break;
                case ItemType.Seed:
                    if(currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1){SetCursorValid();}
                    else {SetCursorInValid();}
                    break;
            }
        }
        else // 因为只标记了可丢弃物品区域，对未标注瓦片执行默认操作
        {
            SetCursorInValid();
        }
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    #endregion


    #region 执行功能(播放动画后)
    private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                //FIXME: 补全物品/工具实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, ItemType.Commodity);
                        break;    
                    case ItemType.HoeTool:
                        GridMapManager.Instance.SetFarmGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //TODO:加音效
                        break;
                    case ItemType.WaterTool:
                        GridMapManager.Instance.SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        break; 
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID,currentTile);
                        //更新背包内种子数量
                        EventHandler.CallDropItemEvent(itemDetails.itemID,mouseWorldPos,itemDetails.itemType);
                        break; 
                }

                // 更新瓦片信息
                GridMapManager.Instance.UpdataTileDetails(currentTile);
            }
        }
    #endregion


    #region Start&Update
    private void Start()
    {
        //获取CursorCanvas下的存放鼠标指针图像的第一个子物体
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        // 初始化为Normal样式的Cursor
        currentSprite = normal;
        SetCursorImage(normal);

        //获取相机
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 图片跟随鼠标
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;

        // 鼠标点击选中物品
        if(!InteractWithUI() && cursorEnable)
        {
            //依据物品类型更新Cursor图片
            SetCursorImage(currentSprite);
            
            //监测鼠标所在格子上物品是否符合Grid Type
            CheckCursorValid();
            
            //监测鼠标所在的格子上物品是否符合Use Radius
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }
    }

    #endregion

}
