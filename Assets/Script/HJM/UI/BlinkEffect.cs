using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BlinkEffect : MonoBehaviour
{
    [Header("깜빡임 적용할 이미지들")]
    public List<Image> targetImages = new List<Image>();

    [Header("깜빡임 적용할 텍스트들")]
    public List<TextMeshProUGUI> targetTexts = new List<TextMeshProUGUI>();

    [Header("깜빡임 설정")]
    public float blinkSpeed = 2f; // 초당 깜빡이는 횟수
    private bool isBlinking = true;

    private void Start()
    {
        if (targetImages.Count == 0 && targetTexts.Count == 0)
        {
            Debug.LogError("이미지와 텍스트 리스트가 비어있음.");
            return;
        }

        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            // 알파값을 서서히 0으로
            yield return StartCoroutine(FadeAlpha(1f, 0f));
            // 알파값을 서서히 1로
            yield return StartCoroutine(FadeAlpha(0f, 1f));
        }
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha)
    {
        float duration = 1f / blinkSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            SetImagesAlpha(alpha);
            SetTextsAlpha(alpha);

            yield return null;
        }

      
        SetImagesAlpha(endAlpha);
        SetTextsAlpha(endAlpha);
    }

    private void SetImagesAlpha(float alpha)
    {
        foreach (Image img in targetImages)
        {
            if (img != null)
            {
                Color color = img.color;
                color.a = alpha;
                img.color = color;
            }
        }
    }

    private void SetTextsAlpha(float alpha)
    {
        foreach (TextMeshProUGUI text in targetTexts)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
    }
}
