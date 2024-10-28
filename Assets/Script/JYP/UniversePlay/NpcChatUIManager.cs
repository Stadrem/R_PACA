using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;
using UnityEngine.UI;
using WebSocketSharp;

public class NpcChatUIManager : MonoBehaviourPun
{
    public Canvas chatCanvas;
    public TMP_InputField ChatInputField;
    public RectTransform listContent;
    public TMP_Text turnText;
    public GameObject ChatBubblePrefab;
    public ScrollRect scrollRect; 
    public void Start()
    {
        ChatInputField.onSubmit.AddListener(OnSubmitText);
    }

    private void OnSubmitText(string txt)
    {
        PlayUniverseManager.Instance.NpcManager.OnChatSubmit(txt);
        AddChatBubble(
            PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.name,
            PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.name + " : " + txt
        );
        // photonView.RPC("AddChatBubble", RpcTarget.All, "누군가..", txt);
        ChatInputField.text = "";
    }

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

    public void RPC_AddChatBubble(string sender, string text)
    {
        AddChatBubble(sender, text);
        return;
        photonView.RPC("AddChatBubble", RpcTarget.All, sender, text);
    }

    [PunRPC]
    private void AddChatBubble(string sender, string text)
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
}