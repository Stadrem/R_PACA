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
    
    public Vector2 Size => infoPanel.sizeDelta;
    public Vector2 Position => infoPanel.anchoredPosition;
    
    public void SetOnNextButtonClicked(Action action)
    {
        nextButton.onClick.AddListener(() => action());
    }
    
    public void RemoveAllOnNextButtonClicked()
    {
        nextButton.onClick.RemoveAllListeners();
    }
    
    public void SetText(string text)
    {
        infoText.text = text;
    }
    
    
    public void MoveTo(Vector2 pos)
    {
        MoveTo(pos.x, pos.y);
    }
    
    public void MoveTo(float x, float y)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToCoroutine(x, y));
    }
    
    private IEnumerator MoveToCoroutine(float x, float y)
    {
        int time = 0;
        Vector2 startPos = infoPanel.anchoredPosition;
        Vector2 endPos = new Vector2(x, y);
        while (time < animateTime)
        {
            infoPanel.anchoredPosition = EaseInOutExponential(time / animateTime, ref startPos, ref endPos);
            yield return null;
        }
        infoPanel.anchoredPosition = endPos;
    }
    
    private Vector2 EaseInOutExponential(float k, ref Vector2 start, ref Vector2 end)
    {
        k = Mathf.Clamp01(k);
        k = k > 0.5f ? 1 - k : k;
        return new Vector2((end.x - start.x) * k * k + start.x, (end.y - start.y) * k * k + start.y);   
    }
    
}