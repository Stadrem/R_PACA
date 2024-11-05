using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcChatSelectorEntryController : MonoBehaviour
{
    public Color baseColor;
    public Color selectedColor;

    public Color baseTextColor;
    public Color selectedTextColor;

    public Image background;
    public TMP_Text contentText;

    private bool isSelected = false;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (value == isSelected) return;
            isSelected = value;
            if (value) Select();
            else Unselect();
        }
    }

    public void SetText(string text)
    {
    }

    private void Unselect()
    {
        background.color = baseColor;
        contentText.color = baseTextColor;
    }

    private void Select()
    {
        background.color = selectedColor;
        contentText.color = selectedTextColor;
    }
}