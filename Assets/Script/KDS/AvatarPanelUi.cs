using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanelUi : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform childRectTransform;
    public VerticalLayoutGroup parentLayoutGroup;  // Mid 오브젝트에 있는 Vertical Layout Group을 참조

    int originHeight;
    int originWidth;
    int subPanelHeight;
    int sumHeight;

    public void OnClickAvatarPanel()
    {
        if (!childRectTransform.gameObject.activeSelf)
        {
            AvatarPanelParentUi.instance.ResetPanel();

            childRectTransform.gameObject.SetActive(true);


                rectTransform.sizeDelta = new Vector2(500, originHeight + subPanelHeight);

            LayoutRefresh();
        }
        else
        {
            AvatarPanelParentUi.instance.ResetPanel();

            LayoutRefresh();
        }
    }

    private void Start()
    {
        //부모의 rectTransform 찾기
        rectTransform = transform.parent.GetComponentInParent<RectTransform>();

        originHeight = Convert.ToInt32(rectTransform.sizeDelta.y);
        originWidth = Convert.ToInt32(rectTransform.sizeDelta.x);

        //자식의 subPanel 찾기
        childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        subPanelHeight = Convert.ToInt32(childRectTransform.sizeDelta.y);
    }

    void LayoutRefresh()
    {
        // 레이아웃 갱신을 강제하여 높이를 재계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
    }
}