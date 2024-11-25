using System;
using System.Collections;
using Data.Local;
using TMPro;
using UnityEngine;

namespace Tutorials.UI
{
    public class CreateUniverseTutorialStart : CreateUniverseTutorialState
    {
        [SerializeField]
        private CreateUniverseTutorialStateManager manager;

        [SerializeField]
        private RectTransform welcomePanel;

        [SerializeField]
        private InfoPanelController infoPanelController;

        [SerializeField]
        private RectTransform titlePanel;

        [SerializeField]
        private RectTransform genrePanel;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private RectTransform characterSettingPanel;

        [SerializeField]
        private RectTransform buttonNext;


        [SerializeField]
        private string[] infoTexts;

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
            //setActive(false) all
            welcomePanel.gameObject.SetActive(false);
            infoPanelController.gameObject.SetActive(false);
            titlePanel.gameObject.SetActive(false);
            genrePanel.gameObject.SetActive(false);
            contentPanel.gameObject.SetActive(false);
            buttonNext.gameObject.SetActive(false);
        }

        public override void OnStartState()
        {
            state = EState.None;
            ShowNext();
        }

        public override void OnEndState()
        {
            gameObject.SetActive(false);
        }

        private void ShowNext()
        {
            if (state == EState.End)
            {
                manager.Next();
                return;
            }

            state = (EState)((int)state + 1);
            switch (state)
            {
                case EState.Welcome:
                    welcomePanel.gameObject.SetActive(true);
                    break;
                case EState.Info:
                    welcomePanel.gameObject.SetActive(false);
                    infoPanelController.gameObject.SetActive(true);
                    infoPanelController.SetText(infoTexts[0]);
                    break;
                case EState.Title:
                    titlePanel.gameObject.SetActive(true);
                    infoPanelController.SetText(infoTexts[1]);
                    infoPanelController.MoveTo(
                        infoPanelController.Position.x,
                        titlePanel.anchoredPosition.y - infoPanelController.Size.y 
                    );
                    break;
                case EState.Genre:
                    titlePanel.gameObject.SetActive(false);
                    genrePanel.gameObject.SetActive(true);
                    infoPanelController.SetText(infoTexts[2]);
                    break;
                case EState.Content:
                    genrePanel.gameObject.SetActive(false);
                    contentPanel.gameObject.SetActive(true);
                    infoPanelController.SetText(infoTexts[3]);
                    infoPanelController.MoveTo(
                        infoPanelController.Position.x,
                        contentPanel.anchoredPosition.y + contentPanel.sizeDelta.y
                    );
                    break;
                case EState.End:
                    infoPanelController.SetText(infoTexts[4]);
                    contentPanel.gameObject.SetActive(false);
                    characterSettingPanel.gameObject.SetActive(true);
                    
                    infoPanelController.MoveTo(
                        characterSettingPanel.anchoredPosition.x + characterSettingPanel.sizeDelta.x,
                        characterSettingPanel.anchoredPosition.y
                    );
                    break;
            }
        }
    }
}