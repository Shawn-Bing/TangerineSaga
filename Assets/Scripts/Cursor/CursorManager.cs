using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;// 图标类型
    private Sprite currentSprite;   // 存储当前鼠标图片
    private Image cursorImage;      // 获取鼠标图片
    private RectTransform cursorCanvas;// 获取UI类型


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
    private void Start()
    {
        //获取CursorCanvas下的存放鼠标指针图像的第一个子物体
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        // 初始化为Normal样式的Cursor
        currentSprite = normal;
        SetCursorImage(normal);
    }

    private void Update()
    {
        // 图片跟随鼠标
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;

        // 依据物品更新Cursor图片
        SetCursorImage(currentSprite);
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
}
