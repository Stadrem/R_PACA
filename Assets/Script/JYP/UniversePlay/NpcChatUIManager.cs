using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class NpcChatUIManager : MonoBehaviour
{
    public Canvas chatCanvas;
    public TMP_InputField ChatInputField;
    public RectTransform listContent;
    public TMP_Text turnText;
    public GameObject ChatBubblePrefab;
    public ScrollRect scrollRect;

    [Header("Chat Options")] 
    public RectTransform optionsContainer;
    public GameObject selectorEntryPrefab;
    public List<NpcChatSelectorEntryController> selectorEntries;


    public void SetTurnText(int turn, string text)
    {
        if (text.IsNullOrEmpty())
        {
            turnText.text = $"{turn} 턴";
        }
        else
        {
            turnText.text = $"{turn} 턴 - {text}";
        }
    }

    public void Show()
    {
        chatCanvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        chatCanvas.gameObject.SetActive(false);
    }

    public void AddChatBubble(string sender, string text)
    {
        GameObject chatBubble = Instantiate(ChatBubblePrefab, listContent);
        chatBubble.GetComponent<NpcChatItem>()
            .SetText(
                sender,
                text,
                sender == PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.name
            );
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(0.1f);
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }


    public void SetChattable(bool chattable)
    {
        ChatInputField.interactable = chattable;
    }


    public void ShowChatOptions(Dictionary<string, string> options)
    {
        if (!optionsContainer.gameObject.activeSelf)
            optionsContainer.gameObject.SetActive(true);

        // create
        foreach (var option in options)
        {
            GameObject selectorEntry = Instantiate(selectorEntryPrefab, optionsContainer);
            var optionText = $"{option.Key} : {option.Value}";
            selectorEntry.GetComponent<NpcChatSelectorEntryController>().SetText(optionText);
            selectorEntries.Add(selectorEntry.GetComponent<NpcChatSelectorEntryController>());
        }
    }
}