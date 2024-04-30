using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.Search;

public class ItemEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();//创建并初始化左侧的列表
    private VisualTreeAsset itemRowTemplate;//实例化模板
    private ListView itemListView;//左侧ListView变量
    private ScrollView itemDetailsSection;//右侧详情区域变量
    private ItemDetails activeItem;//存放 选择item时在右侧显示具体信息 的变量
    private VisualElement iconPreview;//存放icon预览(General下方左侧大图) 变量
    private Sprite defaultIcon;//存放默认Icon

    [MenuItem("Shawn/ItemEditor")]//对编辑器做更改，修改工具路径


    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Instantiate UXML 启用UI Builder中创建的界面
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        //(因为没挂载到物体上)用绝对路径获取模板数据
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        //设定默认Icon
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Shawnnn/Art/Items/Icons/icon_Sword2.png");

        //获取各个组件并赋值给创建的变量
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");//左侧ListView
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");//右侧ItemDetails
        itemDetailsSection.Q<EnumField>("ItemType").Init(ItemType.Seed);//详情的Type
        iconPreview = itemDetailsSection.Q<VisualElement>("icon");//右侧详情大图

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
                { e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture; }
                e.Q<Label>("Name").text = itemList[i] == null ? "Item does't exist" : itemList[i].itemName;

            }
        };
        itemListView.fixedItemHeight = 48;//固定左侧列表图标高度
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        //若选中某项，就获取详情
        itemListView.selectionChanged += OnListSelectionChange;
        //没选择时候(默认)设为不可见
        itemDetailsSection.visible = false;
            
        }

    // 当选中某一个item的时候的回调
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        if (selectedItem != null)
        {
            //First方法需要：System.Linq
            activeItem = (ItemDetails)selectedItem.First();
            if (activeItem != null)
            {
                GetItemDetails();
                itemDetailsSection.visible = true;//选择就显示
            }
            else
            {
                itemDetailsSection.visible = false;
            }
        }
    }

    // 绑定item内容
    private void GetItemDetails()
    {
        // 用标记为脏数据的方式实现面板中数据的更改并同步到物品数据库中
        itemDetailsSection.MarkDirtyRepaint();
        
        //链接itemID
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemID;
        // 回调函数，保证面板上的修改能同步到创建好的ItemDataList_SO(物品数据库)中
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback((evt) =>
        {
            activeItem.itemID = evt.newValue;//赋值
        });
        
        // 链接itemName
        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();//右侧更新名称和图标时左侧重新绘制
        });

        // 链接Type
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        // 链接icon(预览)
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<UnityEditor.UIElements.ObjectField>("ItemIcon").value = activeItem.itemIcon; // 图片赋值
        itemDetailsSection.Q<UnityEditor.UIElements.ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;//临时变量存放新Icon
            activeItem.itemIcon = newIcon;
            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild(); // 修改图片时，重新刷新ListView
        });
        
    }
}
