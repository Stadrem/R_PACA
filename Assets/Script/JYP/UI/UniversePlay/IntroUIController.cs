using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniversePlay;
using ViewModels;

namespace UI.UniversePlay
{
    public class IntroUIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text introText;

        private RectTransform rectTransform;

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;


        private bool isIntroShowing = false;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name != "WaitingScene" && !string.IsNullOrEmpty(ViewModel.IntroMessage) && !isIntroShowing)
            {
                isIntroShowing = true;
                SetIntroText(ViewModel.IntroMessage);
            }
        }

        public void SetIntroText(string text)
        {
            StartCoroutine(AnimateIntroText(text));
        }

        private IEnumerator AnimateIntroText(string text)
        {
            introText.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                introText.text += text[i];
                yield return new WaitForSeconds(0.06f);
            }

            yield return new WaitForSeconds(3f);
            yield return AnimateOut();
        }

        private IEnumerator AnimateOut()
        {
            float elapsedTime = 0;
            var startPos = rectTransform.anchoredPosition;
            var endPos = rectTransform.anchoredPosition;
            endPos.y += rectTransform.rect.height;
            while (elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / 0.5f);
                yield return null;
                ;
            }
            Destroy(gameObject, 0.1f);
        }
    }
}