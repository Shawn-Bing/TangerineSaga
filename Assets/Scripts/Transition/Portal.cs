using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Transition
{
    public class Portal : MonoBehaviour
    {
        [SceneName]//使用Scene Name Attribute
        //TODO：引擎中赋值
        public string sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            //TODO：引擎中给玩家加上Player标签
            if (other.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}