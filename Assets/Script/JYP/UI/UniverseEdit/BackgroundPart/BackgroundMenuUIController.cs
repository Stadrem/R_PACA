using System;
using TMPro;
using UnityEngine;

public class BackgroundMenuUIController : MonoBehaviour
{
    public RectTransform backgroundMenuContainer;
    public TMP_Text menuButtonText;

    [SerializeField] private BackgroundPartLinkManager backgroundPartLinkManager;
    [SerializeField] private LinkedBackgroundPart linkedBackgroundPart;
    private bool isMenuOpen = false;


    private void Start()
    {
    }

    public void OnDetailButtonClicked()
    {
        CloseMenuUI();
        backgroundPartLinkManager.ShowDetailView(linkedBackgroundPart);
    }


    public void OnMenuButtonClick()
    {
        if (isMenuOpen)
        {
            CloseMenuUI();
        }
        else
        {
            ShowMenuUI();
        }
    }

    private void ShowMenuUI()
    {
        
        backgroundMenuContainer.gameObject.SetActive(true);
        isMenuOpen = true;
        menuButtonText.text = "메뉴\n닫기";
    }

    private void CloseMenuUI()
    {
        backgroundMenuContainer.gameObject.SetActive(false);
        isMenuOpen = false;
        menuButtonText.text = "메뉴\n열기";
    }
}