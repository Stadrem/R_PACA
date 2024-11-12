﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EscUiManager : MonoBehaviour
{
    //싱글톤
    public static EscUiManager instance;

    public static EscUiManager Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Canvas_Esc");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<EscUiManager>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {   
            Destroy(gameObject);
        }
    }

    public Slider bgmSlider;
    public Slider sfxSlider;

    public void EnableCanvas()
    {
        gameObject.SetActive(true);
    }

    public void OnClickGameExit()
    {
        Application.Quit();
    }
}
