using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class NpcChatManager : MonoBehaviour
{
    public Canvas chatCanvas;
    public TMP_InputField ChatInputField;
    public RectTransform listContent;

    public GameObject ChatBubblePrefab;
    public StringBuilder ChatLog;
    
    public void Start()
    {
        ChatLog = new StringBuilder();
        ChatInputField.onSubmit.AddListener(OnSubmitText);
    }

    private void OnSubmitText(string txt)
    {
        ShowChatBubble(txt);
        ChatInputField.text = "";
    }

    public void Show()
    {
        chatCanvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        chatCanvas.gameObject.SetActive(false);
    }

    public void ShowChatBubble(string text)
    {
        ChatLog.AppendLine(text);
        GameObject chatBubble = Instantiate(ChatBubblePrefab, listContent);
        chatBubble.GetComponent<NpcChatItem>()
            .SetText(
                "누군가..",
                text,
                Random.Range(0, 2) == 0
            );

    }
}