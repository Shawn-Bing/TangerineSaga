using UnityEngine;

namespace T_Saga.Herbal
{
    public class HerbalManager : Singleton<HerbalManager>
    {
        //TODO: 引擎获取SO文件
        public HerbalDataList_SO herbalData;
        //TODO: 为每个场景都添加父级管理物品
        private Transform herbalParent;
        private Grid currentGrid;
        private Season currentSeason;

        #region 注册事件
        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            HerbalDetails currentHerbalSeed = GetHerbalSeedDetails(ID);
            // 首次种植判断
            if (currentHerbalSeed != null && CheckSeasonAvailable(currentHerbalSeed) && tileDetails.seedItemID == -1)
            {
                // 设定此处的种子类型并开始计时
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;
                // 生成农作物
                ShowHerbalPlant(tileDetails, currentHerbalSeed);
            }
            else if (tileDetails.seedItemID != -1)  //用于刷新地图
            {
                // 刷新地图后，重新显示带有之前数据的农作物
                ShowHerbalPlant(tileDetails, currentHerbalSeed);
            }
        }

        private void OnAfterSceneLoadedEvent()
        {
            //获取父级物体
            currentGrid = FindObjectOfType<Grid>();
            herbalParent = GameObject.FindWithTag("HerbalParent").transform;
        }

        private void OnGameDayEvent(int day, Season season)
        {
            //获取当前季节
            currentSeason = season;
        }
        #endregion

        /// <summary>
        /// 通过物品ID查找种子信息
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns></returns>
        public HerbalDetails GetHerbalSeedDetails(int ID)
        {
            return herbalData.herbalDetailsList.Find(c => c.seedItemID == ID);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="herbal">种子信息</param>
        /// <returns></returns>
        private bool CheckSeasonAvailable(HerbalDetails herbal)
        {
            for (int i = 0; i < herbal.seasons.Length; i++)
            {
                if (herbal.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 显示作物(附带刷新地图)
        /// </summary>
        /// <param name="tileDetails">当前Tile信息</param>
        /// <param name="currentHerbalSeed">当前种下的种子</param>
        private void ShowHerbalPlant(TileDetails tileDetails, HerbalDetails currentHerbalSeed)
        {
            // 获取当前作物成长周期&阶段
            int growthStages = currentHerbalSeed.growthDays.Length;
            int currentStage = 0;
            int dayCounter = currentHerbalSeed.TotalGrowthDays;

            // (倒序计算)更新当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                // 地图刷新时（比如在另一场景度过n天，回到该场景时）更新成长阶段
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= currentHerbalSeed.growthDays[i];
            }

            // 获取当前成长阶段的Prefab
            GameObject cropPrefab = currentHerbalSeed.growthPrefabs[currentStage];
            Sprite cropSprite = currentHerbalSeed.growthSprites[currentStage];

            // 使作物固定在Tile中间（要修改Sprite锚点）
            Vector3 pos = new Vector3(tileDetails.girdX + 0.5f, tileDetails.gridY + 0.5f, 0);

            // 生成作物
            GameObject herbalInstance = Instantiate(cropPrefab, pos, Quaternion.identity, herbalParent);
            
            // 添加图片
            herbalInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
        }
    }
}