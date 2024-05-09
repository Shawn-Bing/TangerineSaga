using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Map
{
    //所有Manager都要挂载到Persistant Scene object上
    public class GridMapManager : MonoBehaviour
    {
        [Header("地图")]
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
    }
}