using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HerbalDataList_SO", menuName = "Herbal/HerbalDataList")]
public class HerbalDataList_SO : ScriptableObject {
    public List<HerbalDetails> herbalDetailsList;
}
