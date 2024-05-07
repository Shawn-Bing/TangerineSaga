using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Transition
{
    public class Portal : MonoBehaviour
    {
        public string sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}