﻿using System.ComponentModel;
using Tutorials;
using UnityEngine;
using ViewModels;

public class CreateUniverseTutorialCharacters : CreateUniverseTutorialState
{
    [SerializeField]
    private CreateUniverseTutorialStateManager manager;

    [SerializeField]
    private InfoPanelController infoPanelController;

    [SerializeField]
    [TextArea(3, 10)]
    private string[] infoTexts;

    [SerializeField]
    private UniverseCharactersEditController universeCharactersEditController;

    [SerializeField]
    private RectTransform directorImage;

    private enum EState
    {
        None = -1,
        CharacterSetting,
        CharacterList,
        End
    }

    private EState state = EState.None;

    private UniverseEditViewModel ViewModel => ViewModelManager.Instance.UniverseEditViewModel;

    public override void OnStartState()
    {
        gameObject.SetActive(true);
        infoPanelController.SetOnNextButtonClicked(ShowNext);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        
        universeCharactersEditController.OnBackgroundButtonClicked += ShowNext;
        state = EState.None;
        ShowNext();
    }

    public override void OnEndState()
    {
        infoPanelController.RemoveAllOnNextButtonClicked();
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        universeCharactersEditController.OnBackgroundButtonClicked -= ShowNext;
        gameObject.SetActive(false);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.Characters))
        {
            if (ViewModel.Characters.Count > 0 && state == EState.CharacterSetting)
                ShowNext();
        }
    }

    private void ShowNext()
    {
        if (state == EState.End)
        {
            manager.Next();
            return;
        }

        Debug.Log($"ShowNext_Chracter - {state} -> {state + 1}");
        state++;

        switch (state)
        {
            case EState.CharacterSetting:
                infoPanelController.SetText(infoTexts[0], hideNextButton: true);
                directorImage.anchoredPosition = new Vector3(-288, 387, 0);
                break;
            case EState.CharacterList:
                infoPanelController.SetText(infoTexts[1]);
                directorImage.anchoredPosition = new Vector3(350, 387, 0);
                break;
            case EState.End:
                infoPanelController.SetText(infoTexts[2], hideNextButton: true);
                directorImage.anchoredPosition = new Vector3(-772, 93, 0);
                break;
        }
    }
}