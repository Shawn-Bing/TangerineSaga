using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T_Saga.Map;

//用于预先在场景中生成定制阶段的物品，如可以砍的树、杂草等
namespace T_Saga.Herbal
{
    public class HerbalGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void OnEnable()
        {
            EventHandler.GenerateHerbEvent += GenerateHerb;
        }

        private void OnDisable()
        {
            EventHandler.GenerateHerbEvent -= GenerateHerb;
        }

        private void GenerateHerb()
        {
            // 获取坐标
            Vector3Int herbGridPos = currentGrid.WorldToCell(transform.position);

            if (seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(herbGridPos);

                if (tile == null)
                {
                    tile = new TileDetails();
                    tile.girdX = herbGridPos.x;
                    tile.gridY = herbGridPos.y;
                }

                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}