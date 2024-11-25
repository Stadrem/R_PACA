using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundNameDisplayUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private RectTransform backgroundNameDisplayContainer;

    [SerializeField]
    private TMP_Text backgroundNameDisplayText;

    [Header("Control")]
    [SerializeField]
    private float openCloseAnimSec = 0.5f;

    [SerializeField]
    private float displayDuration = 3f;

    private void Start()
    {
#if UNITY_EDITOR
        IEnumerator TestBackgroundNameDisplay()
        {
            yield return new WaitForSeconds(1f);
            ShowBackgroundName("Test Background Name");
        }
        if (SceneManager.GetActiveScene().name == "UI_TestScene")
        {
            StartCoroutine(TestBackgroundNameDisplay());
        }
#endif
    }

    /// <summary>
    /// 화면 상단에 텍스트를 표시합니다. 일정 시간 후 사라집니다.
    /// </summary>
    /// <param name="text">상단에 표시할 텍스트</param>
    /// <param name="inAndOutDuration">UI가 나오고 들어가는 Animation이 각각 실행되는 시간</param>
    /// <param name="newDisplayDuration">UI가 가만히 유지되어 표시되는 시간</param>
    public void ShowHeaderText(string text, float inAndOutDuration = 0.5f, float newDisplayDuration = 3f)
    {
        this.displayDuration = newDisplayDuration;
        this.openCloseAnimSec = inAndOutDuration;
        StartCoroutine(CoShowBackgroundName(text));
    }
    
    

    public void ShowBackgroundName(string backgroundName)
    {
        StartCoroutine(CoShowBackgroundName(backgroundName));
    }

    private IEnumerator CoShowBackgroundName(string backgroundName)
    {
        backgroundNameDisplayText.text = backgroundName;
        backgroundNameDisplayContainer.gameObject.SetActive(true);
        yield return StartCoroutine(AnimateInAndOut(isIn: true));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(AnimateInAndOut(isIn: false));
        backgroundNameDisplayContainer.gameObject.SetActive(false);
        Destroy(gameObject);
    }


    private IEnumerator AnimateInAndOut(bool isIn)
    {
        float start = isIn ? 0 : 1;
        float target = isIn ? 1 : 0;
        float elapsedTime = 0f;
        float totalChange = target - start;
        var displayWidth = Screen.width / 2;
        if (isIn)
        {
            backgroundNameDisplayText.rectTransform.anchoredPosition = new Vector2(
                0,
                backgroundNameDisplayText.rectTransform.anchoredPosition.y
            );
        }
        else
        {
            backgroundNameDisplayText.rectTransform.anchoredPosition = new Vector2(
                -displayWidth,
                backgroundNameDisplayText.rectTransform.anchoredPosition.y
            );
        }

        backgroundNameDisplayContainer.localScale = new Vector3(1, start, 1);
        while (elapsedTime < openCloseAnimSec)
        {
            elapsedTime += Time.deltaTime;
            var newY = start + totalChange * (elapsedTime / openCloseAnimSec);
            backgroundNameDisplayContainer.localScale = new Vector3(1, newY, 1);
            //set Position X of text
            var newX = GetEaseOutCirc(
                isIn ? 0 : displayWidth,
                isIn ? displayWidth : 2*displayWidth,
                elapsedTime / openCloseAnimSec
            );
            backgroundNameDisplayText.rectTransform.anchoredPosition = new Vector2(
                newX,
                backgroundNameDisplayText.rectTransform.anchoredPosition.y
            );
            yield return null;
        }

        backgroundNameDisplayContainer.localScale = new Vector3(1, target, 1);
    }
    
    private float GetEaseOutCirc(float start, float end, float t)
    {
        t--;
        return end * Mathf.Sqrt(1 - t * t) + start;
    }
    
}