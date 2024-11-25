using System.ComponentModel;
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
    private string[] infoTexts;


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
        state = EState.None;
        ShowNext();
    }

    public override void OnEndState()
    {
        infoPanelController.RemoveAllOnNextButtonClicked();
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        gameObject.SetActive(false);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.Characters))
        {
            if (ViewModel.Characters.Count > 0 && state == EState.CharacterList)
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

        state = (EState)((int)state + 1);

        switch (state)
        {
            case EState.CharacterSetting:
                infoPanelController.SetText(infoTexts[0], hideNextButton: true);
                state = EState.CharacterList;
                break;
            case EState.CharacterList:
                infoPanelController.SetText(infoTexts[1]);
                state = EState.End;
                break;
            case EState.End:
                infoPanelController.SetText(infoTexts[2], hideNextButton: true);
                break;
        }
    }
}