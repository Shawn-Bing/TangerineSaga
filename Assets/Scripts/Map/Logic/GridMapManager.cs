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
        
        // 字典存放场景名，坐标和对应的瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                //将所有地图数据初始化
                InitTileDetailsDict(mapData);
            }
        }

        #region 注册函数方法
        private void OnEnable() {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        }
        private void OnDisable() {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        }
        private void OnAfterSceneLoadedEvent()
        {
            // 获取锄地、浇水Tilemap
            farmTilemap = GameObject.FindWithTag("FarmLand").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindWithTag("FarmWater").GetComponent<Tilemap>();
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

                // 生成字典关键词
                string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + mapData.SceneName;

                // 根据关键词返回Tile信息
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                // 为Bool变量重新赋值(刷新)
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
    }
}