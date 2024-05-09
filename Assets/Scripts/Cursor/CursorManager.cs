using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    [Header("指针图像")]
    public Sprite normal, tool, seed, item;// 图标类型
    private Sprite currentSprite;   // 存储当前鼠标图片
    private Image cursorImage;      // 获取鼠标图片
    private RectTransform cursorCanvas;// 获取UI类型
    
    [Header("区域检测")]
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;//场景加载完毕之前禁用Cursor

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
    }

    // 物品选中事件
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if(!isSelected)
        {
            currentSprite = normal;//取消选择切换回Normal Cursor
        }
        else
        {
            //TODO::添加所有类型对应Cursor图片
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
        currentGrid = FindObjectOfType<Grid>();
        cursorEnable = true;//加载完之后再启用鼠标
    }

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

        // 依据物品更新Cursor图片
        if(!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
        }
        else
        {
            SetCursorImage(normal);
        }
    }

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

    /// <summary>
    /// 监测鼠标是否可用
    /// 返回网格的世界坐标&网格坐标
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        //这里减去了Main Camera和屏幕的距离
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        
        //测试输出
        Debug.Log("WorldPos:" + mouseWorldPos + "\n" + "GridPos:" + mouseGridPos);
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
}
