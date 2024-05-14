using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace T_Saga.Map
{
    //所有Manager都要挂载到Persistant Scene object上
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("切换到锄地浇水瓦片")]
        //TODO：引擎中赋值
        public RuleTile farmTile;
        public RuleTile waterTile;
        private Tilemap farmTilemap;
        private Tilemap waterTilemap;

        [Header("地图信息")]
        //TODO:引擎中赋值
        public List<MapData_SO> mapDataList;
        
        //存放当前季节
        private Season currentSeason;
        
        // 字典存放场景名，坐标和对应的瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        // 创建一个字典判断场景是否为首次加载
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                // 将所有地图标记为首次加载
                firstLoadDict.Add(mapData.SceneName, true);
                // 将所有地图数据初始化
                InitTileDetailsDict(mapData);
            }
        }

        #region 注册函数方法
        private void OnEnable() {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;//刷新重复收割作物
        }
        private void OnDisable() {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }
        private void OnAfterSceneLoadedEvent()
        {
            // 获取锄地、浇水Tilemap
            farmTilemap = GameObject.FindWithTag("FarmLand").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindWithTag("FarmWater").GetComponent<Tilemap>();

            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                // 预生成一些作物
                EventHandler.CallGenerateHerbEvent();
                // 切换为非首次加载
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            
            // 写入地图数据（Tiles类型）也可以
            // ShowMapDetails(SceneManager.GetActiveScene().name);
            // 还是用Refresh罢！
            RefreshMap();
        }

        /// <summary>
        /// 每天刷新地图网格信息
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;

            foreach (var tile in tileDetailsDict)
            {
                // 每天清空浇水数据
                if (tile.Value.daysSinceWatered > -1)
                {tile.Value.daysSinceWatered = -1;}
                // 每天增加锄地日期
                if (tile.Value.daysSinceDug > -1)
                {tile.Value.daysSinceDug++;}
                
                // n天不锄地且没种作物时恢复为普通土地
                if (tile.Value.daysSinceDug > Settings.maxFarmLandIdleDay && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                // 种下作物时开始计时
                if (tile.Value.seedItemID != -1)
                {tile.Value.growthDays++;}
            }

            RefreshMap();
        }
        #endregion

        // 生成地图信息字典（可刷新）
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperties tileProperties in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = tileProperties.tileCoordinate.x,
                    gridY = tileProperties.tileCoordinate.y
                };

                // 生成字典关键词，组合为第X格瓦片+第Y格瓦片+场景名称
                string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + mapData.SceneName;

                // 根据关键词返回Tile信息
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                // 为Bool变量重新赋值(刷新)
                // TileDetails中包含了瓦片类型
                switch (tileProperties.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperties.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperties.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperties.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperties.boolTypeValue;
                        break;
                }

                // 写入Details数据
                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 由Key返回瓦片信息
        /// </summary>
        /// <param name="key">xy坐标+地图名字</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }

        /// <summary>
        /// 由鼠标的网格坐标返回给Cursor Manager瓦片信息，切换Cursor样式
        /// 因此要改此脚本为Singleton
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }

        /// <summary>
        /// 切换为锄地瓦片
        /// </summary>
        /// <param name="tile">存坐标</param>
        public void SetFarmGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (farmTilemap != null)
                farmTilemap.SetTile(pos, farmTile);
        }

        /// <summary>
        /// 切换为浇水瓦片
        /// </summary>
        /// <param name="tile">存坐标</param>
        public void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (waterTilemap != null)
                waterTilemap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// 切换瓦片之后，更新瓦片信息到字典中
        /// </summary>
        /// <param name="tileDetails">瓦片类型</param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 功能1.加载场景(首次)
        /// 功能2.（切换场景）加载场景后，将原有地图数据写入该场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void ShowMapDetails(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetFarmGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if(tileDetails.seedItemID > -1)
                       EventHandler.CallPlantSeedEvent(tileDetails.seedItemID,tileDetails);
                }
            }
        }

        /// <summary>
        /// 刷新地图瓦片数据
        /// </summary>
        private void RefreshMap()
        {
            // 清空瓦片
            if(farmTilemap != null)
                {farmTilemap.ClearAllTiles();}
            if(waterTilemap != null)
                {waterTilemap.ClearAllTiles();}

            // 清空作物以便重载
            foreach (var crop in FindObjectsOfType<Herb>())
            {
                Destroy(crop.gameObject);
            }
            
            // 重新载入瓦片信息&瓦片种下的种子
            ShowMapDetails(SceneManager.GetActiveScene().name);
        }
    }
}