using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace T_Saga.Herbal
{
    public class ReapInRange : MonoBehaviour
    {
        private HerbalDetails herbalDetails;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        // 初始化数据
        public void InitHerbalData(int ID)
        {
            Debug.Log("InitHerbalData started");
            herbalDetails = HerbalManager.Instance.GetHerbalSeedDetails(ID);
            if (herbalDetails == null)
            {
                Debug.LogError("herbalDetails is null after InitHerbalData");
            }
            else
            {
                Debug.Log("InitHerbalData completed");
            }
        }



        /// <summary>
        /// 实现收获杂草，生成果实(参考Herb)
        /// </summary>
        public void SpawnReapableItems()
        {
            // 确保herbalDetails不是null
            if (herbalDetails == null)
            {
                Debug.LogError("HerbalDetails is null");
                return;
            }
            
            for (int i = 0; i < herbalDetails.producedItemID.Length; i++)
            {
                int spawnAmount = 0;
                if(herbalDetails.producedMinAmount[i] == herbalDetails.producedMaxAmount[i])
                {
                    spawnAmount = herbalDetails.producedMinAmount[i];
                }
                else
                {
                    spawnAmount = Random.Range(herbalDetails.producedMinAmount[i], herbalDetails.producedMaxAmount[i] + 1);
                }

                for (int j = 0; j < spawnAmount; j++)
                {
                    if (herbalDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPlayerPositionEvent(herbalDetails.producedItemID[i]);
                    }
                    else
                    {
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, herbalDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-herbalDetails.spawnRadius.y, herbalDetails.spawnRadius.y), 0);
                        EventHandler.CallInstantiateItemInScene(herbalDetails.producedItemID[i], spawnPos);
                    }
                }
            }
        }
    }

}