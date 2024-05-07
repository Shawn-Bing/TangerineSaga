using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;
    GUIContent[] sceneNames;// 获取场景名称，存储为数组

    readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0) return;

        if (sceneIndex == -1)
            GetSceneNameArray(property);
 
        int oldIndex = sceneIndex;

        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);

        // 切换新场景后重新赋值
        if (oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;
        // 初始化数组
        sceneNames = new GUIContent[scenes.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            // 场景名称存储为字符串
            string path = scenes[i].path;
            string[] splitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";

            if (splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                // 没有这个名称的场景
                sceneName = "(Deleted Scene)";
            }
            sceneNames[i] = new GUIContent(sceneName);
        }

        // 如果BuildSettings没有场景
        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check Your Build Settings") };
        }

        // 防止写程序时候没有初始化Scene Name
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                // 遍历检查已有场景和输入的场景是否一致
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (nameFound == false)
            {
                //返回第一个场景
                sceneIndex = 0;
            }
        }
        else
        {
            //返回第一个场景
            sceneIndex = 0;
        }

        //下拉选框之后返回一个值
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
#endif