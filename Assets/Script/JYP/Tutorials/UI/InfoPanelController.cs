using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    [SerializeField]
    TMP_Text infoText;

    [SerializeField]
    RectTransform infoPanel;

    [SerializeField]
    Button nextButton;

    [SerializeField]
    float animateTime = 0.5f;

    [SerializeField]
    Vector2 startPos;

    [SerializeField]
    private Vector2 middlePos;

    [SerializeField]
    Vector2 endPos;

    public void SetOnNextButtonClicked(Action action)
    {
        nextButton.onClick.AddListener(() => action());
    }

    public void RemoveAllOnNextButtonClicked()
    {
        nextButton.onClick.RemoveAllListeners();
    }

    public void SetText(string text, bool hideNextButton = false)
    {
        Debug.Log($"text: {text}");
        nextButton.gameObject.SetActive(!hideNextButton);
        StartCoroutine(MoveCoroutine(text));
    }

    private IEnumerator MoveCoroutine(string text)
    {
        yield return MoveToCoroutine(endPos.x, endPos.y);
        infoPanel.anchoredPosition = startPos;
        infoText.text = text;
        yield return MoveToCoroutine(middlePos.x, middlePos.y);
    }

    private IEnumerator MoveToCoroutine(float x, float y)
    {
        float time = 0;
        Vector2 srcPos = infoPanel.anchoredPosition;
        Vector2 destPos = new Vector2(x, y);
        while (time < animateTime)
        {
            time += Time.deltaTime;
            infoPanel.anchoredPosition = Vector2.Lerp(srcPos, destPos, time / animateTime);
            yield return null;
        }
        infoPanel.anchoredPosition = destPos;
    }
}