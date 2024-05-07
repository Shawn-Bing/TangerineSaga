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

    /// <summary>
    /// 注册设置边界方法，调整执行顺序
    /// </summary>
    private void OnEnable() {
        EventHandler.AfterSceneLoadEvent += SetConfinerShape;
    }
    private void OnDisable() {
        EventHandler.AfterSceneLoadEvent -= SetConfinerShape;
    }
}
