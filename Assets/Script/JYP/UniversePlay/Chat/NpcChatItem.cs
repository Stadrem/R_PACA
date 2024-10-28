using TMPro;
using UnityEngine;

public class NpcChatItem : MonoBehaviour
{
    public TMP_Text chatText;

    public Color playerColor;
    public Color npcColor;

    public void SetText(string sender, string text, bool isPlayer)
    {
        string color = isPlayer ? ColorUtility.ToHtmlStringRGB(playerColor) : ColorUtility.ToHtmlStringRGB(npcColor);
        text = $"<color=#{color}>{text}</color>";
        chatText.text = text;
    }
}