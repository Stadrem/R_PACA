using System;
using System.Collections;
using System.ComponentModel;
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
        private Action registeredAction;

        private void Start()
        {
            Debug.Log("IntroUIController Start");
            rectTransform = GetComponent<RectTransform>();
            if(ViewModel.IntroMessage != null)
            {
                SetIntroText(ViewModel.IntroMessage);
            }
            else
            {
                ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            }
        }

        private void OnDestroy()
        {
            // ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IntroMessage)
                && !string.IsNullOrEmpty(ViewModel.IntroMessage)
                && !isIntroShowing)
            {
                isIntroShowing = true;
                if (SceneManager.GetActiveScene()
                        .name
                    == "WaitingScene")
                {
                    Debug.Log($"Show intro text: {ViewModel.IntroMessage}");
                    SetIntroText(ViewModel.IntroMessage);
                }

                else if (registeredAction == null)
                {
                    Debug.Log($"Register action to show intro text: {ViewModel.IntroMessage}");
                    registeredAction = () => SetIntroText(ViewModel.IntroMessage);
                }
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