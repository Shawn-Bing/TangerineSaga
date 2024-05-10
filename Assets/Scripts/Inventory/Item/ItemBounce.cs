using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T_Saga.Inventory
{
    //TODO：这个脚本是挂载到父物体的
    public class ItemBounce : MonoBehaviour
    {
        // 获取Transform以控制坐标
        private Transform spriteTsfm;
        // 获取碰撞体以禁用碰撞
        private BoxCollider2D coll;

        [Header("控制物品下落")]
        public float gravity = -3.5f;
        private bool isItemGround;//物品是否落地（控制下落停止）
        private float distance;//距离 要符合Use Radius
        private Vector2 direction;//方向
        private Vector3 targetPos;//位置

        private void Awake()
        {
            // 获取子物体图像
            spriteTsfm = transform.GetChild(0);
            // 获取并关闭碰撞
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        /// <summary>
        /// 生成物品
        /// 在人物锚点上方1.5f生成
        /// </summary>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="dir">方向</param>
        public void InitBounceItem(Vector3 targetPosition, Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = targetPosition;
            distance = Vector3.Distance(targetPosition, transform.position);

            spriteTsfm.position += Vector3.up * 1.5f;
        }

        private void Bounce()
        {
            // 落地条件为 丢出物品Y值 <= 父物体Y值
            isItemGround = spriteTsfm.position.y <= transform.position.y;

            // 没到目标点，移动坐标
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                // X轴移动
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
            }

            if (!isItemGround)
            {
                // Y轴移动
                spriteTsfm.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                //让二者重合并启用碰撞体
                spriteTsfm.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}