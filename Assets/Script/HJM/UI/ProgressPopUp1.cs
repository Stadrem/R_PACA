using System.Collections;
using TMPro;
using UnityEngine;

public class ProgressPopUp1 : MonoBehaviour
{
    public static ProgressPopUp1 Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("UI")]
    [SerializeField]
    private RectTransform container;
    
    [Header("표시할 텍스트")]
    [SerializeField]
    private string comment;

    [SerializeField]
    private TMP_Text progressText;

    [Header("Control")]
    [SerializeField]
    public float openCloseAnimSec = 0.5f;
    public float displayDuration = 2.5f;


    public void Start()
    {
        ShowBattleEnd();
    }

    public void ShowHeaderText(string text, float inAndOutDuration = 0.5f, float newDisplayDuration = 3f)
    {
        this.displayDuration = newDisplayDuration;
        this.openCloseAnimSec = inAndOutDuration;
        StartCoroutine(CoShowProgress(text));
    }

    public void ShowProgress(string progress)
    {
        StartCoroutine(CoShowProgress(progress));
    }

    public void ShowBattleEnd()
    {
        ShowHeaderText(comment, openCloseAnimSec, displayDuration);
    }

    private IEnumerator CoShowProgress(string progress)
    {
        progressText.text = progress;
        container.gameObject.SetActive(true);
        yield return StartCoroutine(AnimateInAndOut(isIn: true));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(AnimateInAndOut(isIn: false));
        container.gameObject.SetActive(false);
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
            progressText.rectTransform.anchoredPosition = new Vector2(
                0,
                progressText.rectTransform.anchoredPosition.y
            );
        }
        else
        {
            progressText.rectTransform.anchoredPosition = new Vector2(
                -displayWidth,
                progressText.rectTransform.anchoredPosition.y
            );
        }

        container.localScale = new Vector3(1, start, 1);
        while (elapsedTime < openCloseAnimSec)
        {
            elapsedTime += Time.deltaTime;
            var newY = start + totalChange * (elapsedTime / openCloseAnimSec);
            container.localScale = new Vector3(1, newY, 1);
            //set Position X of text
            var newX = GetEaseOutCirc(
                isIn ? 0 : displayWidth,
                isIn ? displayWidth : 2 * displayWidth,
                elapsedTime / openCloseAnimSec
            );
            progressText.rectTransform.anchoredPosition = new Vector2(
                newX,
                progressText.rectTransform.anchoredPosition.y
            );
            yield return null;
        }

        container.localScale = new Vector3(1, target, 1);
    }

    private float GetEaseOutCirc(float start, float end, float t)
    {
        t--;
        return end * Mathf.Sqrt(1 - t * t) + start;
    }
}
