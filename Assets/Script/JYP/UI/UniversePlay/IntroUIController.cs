using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UniversePlay;
using ViewModels;

namespace UI.UniversePlay
{
    public class IntroUIController : MonoBehaviourPun
    {
        [SerializeField]
        private TMP_Text introText;
        
        private RectTransform rectTransform;

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

        private bool isIntroShowing = false;
        private Action registeredAction;

        private void Start()
        {
            
            rectTransform = GetComponent<RectTransform>();
            if(ViewModel.IntroMessage != null)
            {
                PlayUniverseManager.Instance.SetIntro(ViewModel.IntroMessage);
            }
            else
            {
              SetIntroText("환영합니다!");  
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