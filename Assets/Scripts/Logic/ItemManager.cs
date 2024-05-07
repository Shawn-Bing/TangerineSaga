using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;//用一个父物体统管全部Prefab

        #region 注册实例化物品事件
        private void OnEnable() {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable() {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }

        //调整获取itemParent执行顺序
        private void OnAfterSceneLoadEvent()
        {
            //获取itemParent
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }
        #endregion

        //由ID生成物体
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;//给出ID即可生成物体
        }
    }
}