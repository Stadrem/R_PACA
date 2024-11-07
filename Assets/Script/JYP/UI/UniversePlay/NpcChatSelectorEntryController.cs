using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcChatSelectorEntryController : MonoBehaviour
{
    public Image background;
    public TMP_Text contentText;
    public Toggle toggle;

    public int Index { get; private set; }

    public void BindData(int index, string text)
    {
        Index = index;
        contentText.text = text;
    }

    public void SetOnValueChanged(Action<int, bool> onValueChanged)
    {
        toggle.onValueChanged.AddListener((t) => onValueChanged(Index, t));
    }
}