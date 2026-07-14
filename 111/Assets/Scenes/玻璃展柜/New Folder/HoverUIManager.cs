using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverUIManager : MonoBehaviour
{
    [Header("悬浮显示名字的面板")]
    public GameObject hoverNamePanel;
    public TextMeshProUGUI hoverText;

    [Header("点击之后详情面板")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;

    void Start()
    {
        hoverNamePanel.SetActive(false);
        detailPanel.SetActive(false);
    }

    //悬浮的时候只显示名字
    public void ShowHoverName(string name)
    {
        hoverText.text = name;
        hoverNamePanel.SetActive(true);
    }
    public void HideHoverName()
    {
        hoverNamePanel.SetActive(false);
    }

    //点击之后弹出详情
    public void ShowDetailInfo(string name, string desc)
    {
        detailNameText.text = name;
        detailDescText.text = desc;
        detailPanel.SetActive(true);
    }
    public void HideDetailPanel()
    {
        detailPanel.SetActive(false);
    }
}