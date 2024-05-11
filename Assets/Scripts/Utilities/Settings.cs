using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    // ItemFade参数
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    // SceneFade 参数
    public const float sceneFadeDuration = 1.5f;

    // 时间系统参数
    public const int MaxmiumYear = 9999;
    public static readonly int CurrentYear = System.DateTime.Now.Year;//获取当前年份
    public const float secondThreshold = 0.012f;//控制计算机计算时间的速度，阈值越小，时间就越快
    public const int secondHold = 59;// 秒数每满 59 进 1
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;
    public const int monthInSeason = 3;

    // 种地参数
    public const int maxFarmLandIdleDay = 5;
}
