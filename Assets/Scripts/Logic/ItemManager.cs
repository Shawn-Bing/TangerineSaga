using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;//用一个父物体统管全部Prefab

        private void OnEnable() {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        }
        private void OnDisable() {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        }

        private void Start() {
            //获取itemParent
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }

        //由ID生成物体
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;//给出ID即可生成物体
        }
    }
}