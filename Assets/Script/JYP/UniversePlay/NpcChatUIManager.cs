using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;

public class NpcChatUIManager : MonoBehaviourPun
{
    public Canvas chatCanvas;
    public TMP_InputField ChatInputField;
    public RectTransform listContent;

    public GameObject ChatBubblePrefab;

    public void Start()
    {
        ChatInputField.onSubmit.AddListener(OnSubmitText);
    }

    private void OnSubmitText(string txt)
    {
        PlayUniverseManager.Instance.NpcManager.OnChatSubmit(txt);
        photonView.RPC("AddChatBubble", RpcTarget.All, "누군가..", txt);
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

    [PunRPC]
    private void AddChatBubble(string sender, string text)
    {
        GameObject chatBubble = Instantiate(ChatBubblePrefab, listContent);
        chatBubble.GetComponent<NpcChatItem>()
            .SetText(
                sender,
                text,
                Random.Range(0, 2) == 0
            );
    }
    
    

    public void SetChattable(bool chattable)
    {
        ChatInputField.interactable = chattable;
    }
}