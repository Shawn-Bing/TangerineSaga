using UnityEngine;
using DG.Tweening;
//为了操作组件改变颜色，必须要有SpriteRenderer组件
[RequireComponent(typeof(SpriteRenderer))]
public class ItemFade : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    //TODO: 能否把颜色写成固定？
    //变半透明
    public void FadeOut()
    {
        Color translucentColor = new Color(1,1,1,Settings.targetAlpha);
        spriteRenderer.DOColor(translucentColor,Settings.itemFadeDuration);
    }
    public void FadeIn()
    {
        Color originColor = new Color(1,1,1,1);
        spriteRenderer.DOColor(originColor,Settings.itemFadeDuration);
    }

    private void Awake() {
        //获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
