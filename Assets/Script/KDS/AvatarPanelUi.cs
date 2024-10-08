using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanelUi : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform childRectTransform;
    public VerticalLayoutGroup parentLayoutGroup;  // Mid ������Ʈ�� �ִ� Vertical Layout Group�� ����

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
        //�θ��� rectTransform ã��
        rectTransform = transform.parent.GetComponentInParent<RectTransform>();

        originHeight = Convert.ToInt32(rectTransform.sizeDelta.y);
        originWidth = Convert.ToInt32(rectTransform.sizeDelta.x);

        //�ڽ��� subPanel ã��
        childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        subPanelHeight = Convert.ToInt32(childRectTransform.sizeDelta.y);
    }

    void LayoutRefresh()
    {
        // ���̾ƿ� ������ �����Ͽ� ���̸� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
    }
}