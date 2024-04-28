using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class ItemEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();//创建并初始化左侧的列表
    private VisualTreeAsset itemRowTemplate;//实例化模板
    private ListView itemListView;//存放VisualElement

    [MenuItem("Shawn/ItemEditor")]//对编辑器做更改，修改工具路径


    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML 启用UI Builder中创建的界面

        //下方代码的另一种写法：
        //var VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        //VisualElement labelFromUXML = VisualTree.Instantiate();
        //root.Add(labelFromUXML);

        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        //因为没挂载到物体上，需要用绝对路径获取模板数据
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        //获取左侧ListView赋值给创建的变量
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");

        //执行加载数据
        LoadDataBase();
        //执行生成列表
        GenerateListView();
    }

    //自制代码要放在下面，否则会导致修改窗口代码失效？ 存疑

    //获取模板数据
    private void LoadDataBase()
    {
        //找类型为ItemDataList_SO的资产并返回一个数组
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");
        if (dataArray.Length > 1)
        {
            //找到第一个元素的路径
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            //依据路径加载数据库
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;//强制转换的另一种写法
        }

        itemList = dataBase.itemDetailsList;
        //标记为脏数据，不标记没法保存到源文件
        EditorUtility.SetDirty(dataBase);

        //测试代码
        //Debug.Log(itemList[0].itemID);
    }

    //生成ListView
    private void GenerateListView()
    {
        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon != null)
                { e.Q<VisualElement>("icon").style.backgroundImage = itemList[i].itemIcon.texture; }
                e.Q<Label>("name").text = itemList[i] == null ? "Item does't exist" : itemList[i].itemName;

            }
        };
        itemListView.fixedItemHeight = 60;//固定左侧列表图标高度
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
    }
}
