using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//TODO:挂载该组件到每一个Grid Properties物体上，为新场景添加Map_SO文件，设置对应Type
[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    //TODO：引擎中赋值
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            //获得组件
            currentTilemap = GetComponent<Tilemap>();

            //每次启动时，刷新Properties
            if (mapData != null)
                mapData.tileProperties.Clear();
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            //关闭时，读入并更新Properties
            UpdateTileProperties();
            
            #if UNITY_EDITOR
            //标脏保存数据到文件中
            if (mapData != null)
                EditorUtility.SetDirty(mapData);
            #endif
        }
    }

    private void UpdateTileProperties()
    {
        //获取实际绘制Tilemap边界
        currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //已绘制范围的左下角坐标
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //已绘制范围的右上角坐标
                Vector3Int endPos = currentTilemap.cellBounds.max;

                //遍历获取全部实际绘制的Tilemap，存入Properties
                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            //写入信息
                            TileProperties newTile = new TileProperties
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };

                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
} 