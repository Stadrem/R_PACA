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
    public void SetUp()
    {
        text_Title.text = set.title;
        icon.sprite = set.sprite;
        text_Description.text = set.description;
        backgroundColor.color = set.color;
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
