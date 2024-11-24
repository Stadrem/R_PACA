using System;
using System.Collections;
using Data.Local;
using UnityEngine;

namespace Tutorials.UI
{
    public class CreateUniverseTutorialStart : MonoBehaviour, ICreateUniverseTutorialState
    {
        [SerializeField]
        private GameObject welcomePanel;

        [SerializeField]
        private GameObject infoPanel;

        [SerializeField]
        private GameObject titlePanel;

        [SerializeField]
        private GameObject genrePanel;

        [SerializeField]
        private GameObject contentPanel;

        [SerializeField]
        private GameObject buttonNext;

        private void Start()
        {
            //setActive(false) all
            welcomePanel.SetActive(false);
            infoPanel.SetActive(false);
            titlePanel.SetActive(false);
            genrePanel.SetActive(false);
            contentPanel.SetActive(false);
            buttonNext.SetActive(false);
        }

        public void OnStartState()
        {
        }

        public void OnEndState()
        {
            tutorialPanel.SetActive(false);
            buttonNext.SetActive(false);
        }

        private IEnumerator Tutorial()
        {
            //show hello
            welcomePanel.SetActive(true);
            //wait next
        }
    }
}