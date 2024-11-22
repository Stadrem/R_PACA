using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using ViewModels;
using WebSocketSharp;

public class NpcChatUIManager : MonoBehaviour
{
    public TMP_InputField ChatInputField;
    public RectTransform listContent;
    public TMP_Text turnText;
    public GameObject ChatBubblePrefab;
    public ScrollRect scrollRect;
    public ToggleGroup selectorToggleGroup;
    public Button finishButton;


    [Header("Chat Options")]
    public RectTransform optionsContainer;

    public GameObject selectorEntryPrefab;
    public List<NpcChatSelectorEntryController> selectorEntries;


    private void Start()
    {
        finishButton.onClick.AddListener(
            () =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PlayUniverseManager.Instance.FinishConversation();
                }
            }
        );
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
        finishButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        finishButton.gameObject.SetActive(false);
        optionsContainer.gameObject.SetActive(false);
    }

    public void AddChatBubble(string sender, string text, bool isPlayer)
    {
        GameObject chatBubble = Instantiate(ChatBubblePrefab, listContent);
        chatBubble.GetComponent<NpcChatItem>()
            .SetCharacterText(
                sender,
                text,
                isPlayer
            );

        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForSeconds(0.1f); // 다음 프레임까지 대기
        scrollRect.ScrollToBottom();
    }


    public void SetChattable(bool chattable)
    {
        ChatInputField.interactable = chattable;
    }


    public void ClearChatOptions()
    {
        foreach (var entry in selectorEntries)
        {
            Destroy(entry.gameObject);
        }

        selectorEntries.Clear();
    }

    public void HideChatOptions()
    {
        optionsContainer.SetParent(null);
        if (optionsContainer.gameObject.activeSelf)
            optionsContainer.gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(optionsContainer);
    }

    public void ShowChatOptions(List<KeyValuePair<string, string>> options)
    {
        if (!optionsContainer.gameObject.activeSelf)
            optionsContainer.gameObject.SetActive(true);
        optionsContainer.SetParent(listContent);
        foreach (var entry in selectorEntries)
        {
            Destroy(entry.gameObject);
        }

        selectorEntries.Clear();
        selectorToggleGroup.SetAllTogglesOff();

        // create
        foreach (var option in options)
        {
            var idx = options.IndexOf(option);
            GameObject selectorEntry = Instantiate(selectorEntryPrefab, optionsContainer);
            var optionText = $"{option.Key} : {option.Value}";
            var controller = selectorEntry.GetComponent<NpcChatSelectorEntryController>();
            controller.BindData(idx, optionText);
            selectorToggleGroup.RegisterToggle(controller.toggle);
            controller.SetOnValueChanged(OnToggleValueChanged);

            selectorEntries.Add(selectorEntry.GetComponent<NpcChatSelectorEntryController>());
        }

        if (options.Count == 1)
        {
            selectorEntries[0].toggle.isOn = true;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(optionsContainer);
        Canvas.ForceUpdateCanvases();
    }

    public void OnToggleValueChanged(int index, bool isOn)
    {
        print($"index: {index}, isOn: {isOn}");
        if (!isOn) return;

        ViewModelManager.Instance.UniversePlayViewModel.NpcChatSelectedIndex = index;


        for (int i = 0; i < selectorToggleGroup.ActiveToggles().Count(); i++)
        {
            if (i == index) continue;
            selectorToggleGroup.ActiveToggles().ElementAt(i).SetIsOnWithoutNotify(false);
        }
    }
}