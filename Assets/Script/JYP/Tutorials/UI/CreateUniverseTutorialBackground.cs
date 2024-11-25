using System;
using Tutorials;
using UnityEngine;
using UniverseEdit;

public class CreateUniverseTutorialBackground : CreateUniverseTutorialState
{
    [Header("Background 로직")]
    [SerializeField]
    private BackgroundCreateUIController backgroundCreateUIController;

    [SerializeField]
    private BackgroundEditUIController backgroundEditUIController;

    [SerializeField]
    private BackgroundPartLinkManager backgroundPartLinkManager;

    [Header("Tutorial 컨트롤")]
    [SerializeField]
    private CreateUniverseTutorialStateManager manager;

    [SerializeField]
    private InfoPanelController infoPanelController;


    [SerializeField]
    [TextArea(3, 10)]
    private string[] infoTexts;

    private enum EState
    {
        None = -1,
        BackgroundStart,
        BackgroundCreate,
        BackgroundDetail,
        BackgroundNpcPlace,
        BackgroundNpcReturnUI,
        BackgroundCreate2,
        BackgroundLink,
        BackToMain,
        End,
    }

    private EState currentState = EState.None;

    public override void OnStartState()
    {
        gameObject.SetActive(true);
        infoPanelController.SetOnNextButtonClicked(ShowNext);
    }

    public override void OnEndState()
    {
        infoPanelController.RemoveAllOnNextButtonClicked();
        gameObject.SetActive(false);
    }

    private void ShowNext()
    {
        if (currentState == EState.End)
        {
            manager.Next();
            return;
        }

        currentState++;

        switch (currentState)
        {
            case EState.None:
                break;
            case EState.BackgroundStart:
                infoPanelController.SetText(infoTexts[0]);
                break;
            case EState.BackgroundCreate:
                infoPanelController.SetText(infoTexts[1]);
                break;
            case EState.BackgroundDetail:
                infoPanelController.SetText(infoTexts[2]);
                break;
            case EState.BackgroundNpcPlace:
                infoPanelController.SetText(infoTexts[3]);
                break;
            case EState.BackgroundNpcReturnUI:
                infoPanelController.SetText(infoTexts[4]);
                break;
            case EState.BackgroundCreate2:
                infoPanelController.SetText(infoTexts[5]);
                break;
            case EState.BackgroundLink:
                infoPanelController.SetText(infoTexts[6]);
                break;
            case EState.BackToMain:
                infoPanelController.SetText(infoTexts[7]);
                break;
            case EState.End:
                infoPanelController.SetText(infoTexts[8]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}