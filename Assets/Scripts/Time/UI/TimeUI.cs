using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// 挂载到TimeUI上的脚本
public class TimeUI : MonoBehaviour
{
    // TODO：在引擎中获取各部分组件
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    // 季节背景 & 季节图集(4个)
    public Image seasonImage;
    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    { 
        // 获取时钟图片
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockBlocks[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameHourEvent += OnGameHourEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameHourEvent -= OnGameHourEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        // 以 XX(小时) : XX(分钟)输出
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameHourEvent(int hour, int day, int month, int year, Season season)
    {
        // 格式：第n年x月x日
        //dateText.text = "第" + year + "年 " + month + "月" + day + "日";
        // 格式：xxx年
        dateText.text = year + "年" + month + "月" + day + "日";

        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);

        DayNightImageRotate(hour);
    }

    /// <summary>
    /// 根据小时切换时间Block显示数量，因为有6块所以每过四小时增加一块 
    /// </summary>
    /// <param name="hour">小时</param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        for (int i = 0; i < clockBlocks.Count; i++)
        {
            if (i <= index - 1)
                clockBlocks[i].SetActive(true);
            else
                clockBlocks[i].SetActive(false);
        }
    }

    /// <summary>
    /// 根据时间旋转昼夜图标
    /// </summary>
    /// <param name="hour">小时</param>
    private void DayNightImageRotate(int hour)
    {
        //旋转Z轴就可以，24小时转360度，每小时转15度，默认设为-90度(晚上)
        var target = new Vector3(0, 0, - 90 + hour * 15 );
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
