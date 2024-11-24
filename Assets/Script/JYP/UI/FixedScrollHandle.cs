using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedScrollHandle : MonoBehaviour
{
    public Scrollbar scrollbar; // 연결할 Scrollbar 오브젝트

    private float size;

    void Start()
    {
        size = 32 / (float)850;
        if (scrollbar != null)
        {
            // 고정 크기 설정
            scrollbar.size = size;
        }
        else
        {
            Debug.LogError("Scrollbar가 연결되지 않았습니다!");
        }
    }

    private void Update()
    {
        // Handle 크기를 유지하려면 반복적으로 size를 고정
        if (scrollbar != null && !Mathf.Approximately(scrollbar.size, size))
        {
            scrollbar.size = size;
        }
    }
}