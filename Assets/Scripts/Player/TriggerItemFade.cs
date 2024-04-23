using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFade : MonoBehaviour
{
    //玩家进入碰撞体时
    private void OnTriggerEnter2D(Collider2D other) {
        //创建数组保存子物体
        ItemFade[] fades = other.GetComponentsInChildren<ItemFade>();

        //遍历数组
        if(fades.Length > 0) 
        {
            //把每个物体都恢复不透明
            foreach(var item in fades)
            {item.FadeOut();}
        }
    }
    //玩家离开碰撞体时
    private void OnTriggerExit2D(Collider2D other) {
        ItemFade[] fades = other.GetComponentsInChildren<ItemFade>();
        if(fades.Length > 0) 
        {
            foreach(var item in fades)
            //半透明化
            {item.FadeIn();}
        }
    }
}
