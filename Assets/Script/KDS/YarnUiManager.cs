using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YarnUiManager : MonoBehaviour
{
    //싱글톤
    public static YarnUiManager instance;

    public YarnHistoryGetSet yarnHistoryGetSet;

    public TMP_Text text_pageNum;

    int pageNum = 1;

    public static YarnUiManager Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Canvas_Yarn");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<YarnUiManager>();

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

    public GameObject page2;
    public GameObject page3;

    public void EnableCanvas()
    {
        gameObject.SetActive(true);
    }

    public void OnClickPage2()
    {
        page2.SetActive(true);
    }

    public void OnClickPage3()
    {
        page3.SetActive(true);
    }

    public void OnClickPrevious()
    {
        pageNum -= 1;

        if (pageNum <= 0)
        {
            pageNum = 1;
        }
        else
        {
            yarnHistoryGetSet.GetYarnCard(pageNum);
        }
        PageUpadte();
    }

    public void OnClickNext()
    {
        pageNum += 1;
        yarnHistoryGetSet.GetYarnCard(pageNum);
        PageUpadte();
    }

    void PageUpadte()
    {
        text_pageNum.text = pageNum + " 페이지";
    }
}
