using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitItem : MonoBehaviour
{
    [Header("展品名称(鼠标悬浮显示)")]
    public string itemName;

    [Header("展品详细介绍(点击后弹出)")]
    [TextArea(6,12)]
    public string itemDescription;
}
