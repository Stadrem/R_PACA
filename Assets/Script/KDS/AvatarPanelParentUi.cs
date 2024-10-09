using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPanelParentUi : MonoBehaviour
{
    public static AvatarPanelParentUi instance;

    private void Awake()
    {
        instance = this;
    }
    RectTransform[] childrenPanel;

    int originHeight;
    int originWidth;

    // Start is called before the first frame update
    void Start()
    {
        childrenPanel = new RectTransform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject temp = transform.GetChild(i).gameObject;

            childrenPanel[i] = temp.GetComponent<RectTransform>();
        }

        originHeight = Convert.ToInt32(childrenPanel[0].sizeDelta.y);
        originWidth = Convert.ToInt32(childrenPanel[0].sizeDelta.x);
    }

    public void ResetPanel()
    {
        foreach (RectTransform child in childrenPanel)
        {
            Transform childA = child.transform.GetChild(0);
            Transform childB = childA.transform.GetChild(0);

            childB.gameObject.SetActive(false);
            child.sizeDelta = new Vector2(originWidth, originHeight);
        }
    }
}
