using TMPro;
using UnityEngine;

public class NpcChatItem : MonoBehaviour
{
    public TMP_Text chatText;

    public Color playerColor;
    public Color npcColor;
    public Color gameMasterColor;

    public void SetCharacterText(string sender, string text, bool isPlayer)
    {
        string color = isPlayer ? ColorUtility.ToHtmlStringRGB(playerColor) : ColorUtility.ToHtmlStringRGB(npcColor);
        text = $"<color=#{color}><b>{sender}</b></color> - {text}";
        chatText.text = text;
    }

    public void SetGameMasterText(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(gameMasterColor);
        text = $"<color=#{color}>{text}</color>";
        chatText.text = text;
        chatText.alignment = TextAlignmentOptions.Center;
    }
}