using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace T_Saga.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        
        //用一个父物体统管全部Prefab
        //TODO：引擎中赋值
        private Transform itemParent;

        // 记录场景Item，存放在一个字典里
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        

        #region 注册实例化物品事件
        private void OnEnable() {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }

        private void OnDisable() {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        }

        // 获取场景物品
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }
        
        //调整获取itemParent执行顺序
        private void OnAfterSceneLoadEvent()
        {
            //TODO:引擎中加标签
            // 获取itemParent
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            // 重建原有物品
            RecreateAllItems();
        }

        #endregion

        //由ID生成物体
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;//给出ID即可生成物体
        }

        /// <summary>
        /// 获得当前场景所有Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            // 获取当前场景物品
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem//生成一个新场景
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };

                currentSceneItems.Add(sceneItem);
            }

            // 更新场景物品
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // 找到数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else
            {
                // 如果是新场景,只添加物品
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 刷新重建当前场景原有物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            // 若当前场景内有物品
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    // 删除所有物品
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    // 重新创建物品
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}