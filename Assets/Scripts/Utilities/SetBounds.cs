using UnityEngine;
using Cinemachine;
public class SetBounds : MonoBehaviour
{
    private void SetConfinerShape()
    {
        //提前在Unity中为Bounds设置Tag为BoundsConfiner
        PolygonCollider2D confinerShape = 
        GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        //获取组件
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        //为Confiner赋值，即设置相机边界
        confiner.m_BoundingShape2D = confinerShape;

        //Runtime切换场景时清除边界缓存
        confiner.InvalidatePathCache();
    }

    private void Start()
    {
        //在游戏开始时运行该函数，设置边界，但要注意切换时不会再次运行，因此还要在之后更改调用
        SetConfinerShape();
    }
}
