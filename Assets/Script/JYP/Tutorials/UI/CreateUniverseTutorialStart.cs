using System;
using System.Collections;
using System.ComponentModel;
using Data.Local;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using ViewModels;

namespace Tutorials.UI
{
    public class CreateUniverseTutorialStart : CreateUniverseTutorialState
    {
        [SerializeField]
        private CreateUniverseTutorialStateManager manager;

        [SerializeField]
        private RectTransform directorImage;
        
        [SerializeField]
        private RectTransform welcomePanel;

        [SerializeField]
        private InfoPanelController infoPanelController;

        [SerializeField]
        [TextArea]
        private string[] infoTexts;
        
        private UniverseEditViewModel ViewModel => ViewModelManager.Instance.UniverseEditViewModel;

        private enum EState
        {
            None = -1,
            Welcome,
            Info,
            Title,
            Genre,
            Content,
            End
        }

        private EState state = EState.None;

        private void Start()
        {
            
        }

        public override void OnStartState()
        {
            Debug.Log("Start");
            gameObject.SetActive(true);
            infoPanelController.SetOnNextButtonClicked(ShowNext);
            state = EState.None;
            ShowNext();
        }

        public override void OnEndState()
        {
            Debug.Log("End");
            infoPanelController.RemoveAllOnNextButtonClicked();
            gameObject.SetActive(false);
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.))
            {
                if (ViewModel.Characters.Count > 0 && state == EState.CharacterList)
                    ShowNext();
            }
        }

        private void ShowNext()
        {
            Debug.Log("ShowNext - " + state);
            if (state == EState.End)
            {
                manager.Next();
                return;
            }

            state = (EState)((int)state + 1);
            switch (state)
            {
                case EState.Welcome:
                    Debug.Log("Welcome");
                    welcomePanel.gameObject.SetActive(true);
                    StartCoroutine(HideWelcomeAfter(3f));
                    break;
                case EState.Info:
                    Debug.Log("Info");
                    infoPanelController.gameObject.SetActive(true);
                    StartCoroutine(HideWelcomePanel());
                    infoPanelController.SetText(infoTexts[0]);
                    break;
                case EState.Title:
                    Debug.Log("Title");
                    directorImage.gameObject.SetActive(true);
                    directorImage.anchoredPosition = new Vector3(-423, 293,0);
                    infoPanelController.SetText(infoTexts[1]);
                    break;
                case EState.Genre:
                    Debug.Log("Genre");
                    directorImage.anchoredPosition = new Vector3(359, 293, 0);
                    infoPanelController.SetText(infoTexts[2]);
                    break;
                case EState.Content:
                    Debug.Log("Content");
                    directorImage.anchoredPosition = new Vector3(-423, 58, 0);
                    infoPanelController.SetText(infoTexts[3]);
                    break;
                case EState.End:
                    Debug.Log("End");
                    directorImage.anchoredPosition = new Vector3(-793, 284, 0);
                    infoPanelController.SetText(infoTexts[4], hideNextButton: true);
                    break;
            }
        }

        private IEnumerator HideWelcomePanel()
        {
            float t = 0;
            var startPos = welcomePanel.anchoredPosition;
            var endPos = new Vector2(0, 0);
            const float animateTime = 0.5f;
            while (t < animateTime)
            {
                t += Time.deltaTime;
                //set position
                var position = Vector2.Lerp(startPos, endPos, t / animateTime);
                welcomePanel.anchoredPosition = position;
                //set scale
                var startScale = new Vector2(1, 1);
                var endScale = new Vector2(0, 0);
                var scale = Vector2.Lerp(startScale, endScale, t / animateTime);
                welcomePanel.localScale = scale;
                yield return null;
            }

            welcomePanel.gameObject.SetActive(false);
        }

        private IEnumerator HideWelcomeAfter(float sec)
        {
            yield return new WaitForSeconds(sec);
            ShowNext();
        }
        
        
        
    }
}