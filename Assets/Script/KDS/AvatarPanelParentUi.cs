﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPanelParentUi : MonoBehaviour
{
    //Panel들 열고 닫는 스크립트. Mid에 부착

    public static AvatarPanelParentUi instance;

    private void Awake()
    {
        instance = this;
    }

    //Panel ABCD 들어갈 자리
    RectTransform[] childrenPanel;

    int originHeight;
    int originWidth;

    // Start is called before the first frame update
    void Start()
    {
        //자식으로 있는 Panel ABCD를 받아옴
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
