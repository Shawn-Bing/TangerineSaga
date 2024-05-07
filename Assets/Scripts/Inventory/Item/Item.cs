using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    public class Item : MonoBehaviour
    {
        //TODO：引擎中赋值
        public int itemID;
        public ItemDetails itemDetails;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;

        public void Init(int ID)
        {
            itemID = ID;
            //获取当前Inventory数据
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);

            if (itemDetails != null)//获取类型可能为空，要加上判空条件
            {
                //设置SpriteRenderer使其显示无描边图
                spriteRenderer.sprite = (itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon);
                //修改碰撞体尺寸和偏移（应对Pivot修改的情况）使其自适应icon
                coll.size = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
        }
        private void Awake()
        {
            //获取子物体SR组件(Item挂载在父物体上)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID != 0)
                Init(itemID);
        }
    }
}