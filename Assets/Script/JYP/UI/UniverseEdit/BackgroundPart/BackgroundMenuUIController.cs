using System;
using TMPro;
using UnityEngine;
using UniverseEdit;

public class BackgroundMenuUIController : MonoBehaviour
{
    public RectTransform backgroundMenuContainer;
    public TMP_Text menuButtonText;

    [SerializeField] private LinkedBackgroundPart linkedBackgroundPart;
    private bool isMenuOpen= false;

    public void OnAddLinkButtonClicked()
    {
        CloseMenuUI();
        BackgroundPartLinkManager.Get().StartLink(linkedBackgroundPart);
    }

    public void OnDeleteLinkButtonClicked()
    {
        BackgroundPartLinkManager.Get().DeleteLink(linkedBackgroundPart);
    }

    public void OnDetailButtonClicked()
    {
        CloseMenuUI();
        BackgroundPartLinkManager.Get().ShowDetailView(linkedBackgroundPart);
    }

    public void OnDeleteBackgroundButtonClicked()
    {
        CloseMenuUI();
        BackgroundPartLinkManager.Get().DeleteBackground(linkedBackgroundPart);
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