using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDetectManager : MonoBehaviour
{
    [Header("悬浮提示UI管理器")]
    public HoverUIManager hoverUIManager;
    
    private ExhibitItem currentSelectItem;

    void Update()
    {
        //发射射线：从鼠标屏幕位置发射
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ExhibitItem item = hit.collider.GetComponent<ExhibitItem>();
            if (item != null)
            {
                //鼠标悬浮在展品上面：显示名字
                if (currentSelectItem != item)
                {
                    currentSelectItem = item;
                    hoverUIManager.ShowHoverName(item.itemName);
                }

                //鼠标左键单击，弹出详情面板
                if (Input.GetMouseButtonDown(0))
                {
                    hoverUIManager.ShowDetailInfo(item.itemName, item.itemDescription);
                }
            }
            else
            {
                //鼠标碰到别的物体，关闭悬浮文字
                currentSelectItem = null;
                hoverUIManager.HideHoverName();
            }
        }
        else
        {
            //鼠标在空白处，关闭悬浮文字
            currentSelectItem = null;
            hoverUIManager.HideHoverName();
    
            //点击空白处关闭详情面板
            if (Input.GetMouseButtonDown(0))
            {
                hoverUIManager.HideDetailPanel();
            }
        }
    }
}