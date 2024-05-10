using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        //TODO:引擎中赋值要产生阴影的物体
        public SpriteRenderer itemSprite;
        private SpriteRenderer itemShadowSprite;

        private void Awake()
        {
            itemShadowSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            itemShadowSprite.sprite = itemSprite.sprite;
            itemShadowSprite.color = new Color(0, 0, 0, 0.4f);
        }

    }
}