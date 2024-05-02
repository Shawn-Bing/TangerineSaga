using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryRepo_SO", menuName = "InventoryRepo_SO")]
public class InventoryRepo_SO : ScriptableObject {
    //包含了所有容器如背包、箱子等的模板
    public List<InventoryItem> itemList;
}