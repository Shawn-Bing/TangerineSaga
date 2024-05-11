using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason;
    private int monthInSeason;
    public bool gameClockPause;// 古希腊掌管游戏暂停的神
    private float tickTime;// 计时器
    private void Awake()
    {
        NewGameTime();
    }

    // 在Enable之后执行，调用更新时间UI
    private void Start() {
        EventHandler.CallGameHourEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
    }

    private void Update()
    {
        // 计时器更新时间
        if (!gameClockPause)
        {
            tickTime += Time.deltaTime;

            if (tickTime >= Settings.secondThreshold)
            {
                tickTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
        
        // FIXME：金手指，按住就加1分钟
        if(Input.GetKey(KeyCode.T))
        {
            for(int i=0;i < 60;i++)
            {
                UpdateGameTime();
            }
        }
        // FIXME：金手指，增加1天,但不能增加月份
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay,gameSeason);
            EventHandler.CallGameHourEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        }
    }

    /// <summary>
    /// 初始化游戏时间系统
    /// </summary>
    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 1;
        gameYear = Settings.CurrentYear;
        gameSeason = Season.春天;
        monthInSeason = Settings.monthInSeason;
    }


    /// <summary>
    /// 更新游戏时间
    /// </summary>
    private void UpdateGameTime()
    {
        gameSecond++;// Second更新

        if (gameSecond > Settings.secondHold)
        {
            // Minute更新
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                // Hour更新
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    // 日期更新
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        // 月份更新
                        gameMonth++;
                        monthInSeason--;
                        gameDay = 1;
                        
                        if (monthInSeason == 0)
                        {
                            //季节更新
                            monthInSeason = Settings.monthInSeason;
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                // 年份更新
                                gameYear++;
                                seasonNumber = 0;
                                gameMonth = 1;
                            }   

                            gameSeason = (Season)seasonNumber;// 赋给枚举量(也就是中文季节名)

                            //给年数加上限(optional)
                            if(gameYear > Settings.MaxmiumYear)
                            {
                                gameYear = (int)2024;
                            }
                        }
                        
                        //每天更新地图类型和农作物成长状态
                        EventHandler.CallGameDayEvent(gameDay,gameSeason);
                    }
                }
                // 呼叫事件，更新游戏日期（日期关联到更上级）
                EventHandler.CallGameHourEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
            }
            // 更新分钟
            EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
        }
        // 测试代码，输出当前时间数据
        //Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute + " Hour: " + gameHour + " Day: " + gameDay + " Month: " + gameMonth + " Season: " + gameSeason + " Year: " + gameYear);
    }
}