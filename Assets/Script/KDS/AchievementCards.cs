using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AchievementCards : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AchievementSet set;

    public TMP_Text text_Title;
    public Image icon;
    public TMP_Text text_Description;
    public Image backgroundColor;

    public GameObject descriptionPopUp;

    //데이터 반영
    public void SetUp(bool unlock)
    {
        //언락 상태면 정상값 입력
        if (unlock) 
        {
            text_Title.text = set.title;
            text_Title.color = new Color32(255, 255, 255, 255);
            icon.sprite = set.sprite;
            icon.color = new Color32(255, 255, 255, 255);
            text_Description.text = set.description;
            backgroundColor.color = set.color;
        }
        //언락 아니면 선택 불가하게
        else
        {
            text_Title.text = set.title;
            text_Title.color = new Color32(128, 128, 128, 128);
            icon.sprite = null;
            icon.color = new Color32(0, 0, 0, 0);
            text_Description.text = set.description;
            backgroundColor.color = Color.black;
        }
    }

    //마우스 포인터 오버 이벤트
    public void OnPointerEnter(PointerEventData eventData)

    {
        descriptionPopUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPopUp.SetActive(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {

    }
}
