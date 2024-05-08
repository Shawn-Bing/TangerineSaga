using UnityEngine;
using TMPro;

public class SinText : MonoBehaviour
{
    public TMP_Text tmp;//TODO:引擎中获取组件

    void Update()
    {
        tmp.ForceMeshUpdate(); //刷新文字Mesh
        var textInfo = tmp.textInfo; //获取文字信息

        for (int i = 0; i < textInfo.characterCount; i++) //循环访问每个文字
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices; //获取每个文字顶点信息

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                //添加动画
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 3f + orig.x * 1.0f) * 2.0f, 0); //对四个顶点进行偏移（使用sin方法实现）
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) //写入
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
