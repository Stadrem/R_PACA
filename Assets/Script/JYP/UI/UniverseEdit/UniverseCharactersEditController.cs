using System.Collections.Generic;
using System.ComponentModel;
using UI.Universe.Edit;
using UnityEngine;
using UnityEngine.UIElements;

public class UniverseCharactersEditController : MonoBehaviour
{
    public VisualTreeAsset characterItemTemplate;

    private UniverseEditViewModel viewModel;
    private CharactersListViewController charactersListController;
    private CharacterCreationController characterCreationController;
    private Button backButton;
    private Label createdDate;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>()
            .rootVisualElement;
        backButton = root.Q<Button>("btn_back");
        createdDate = root.Q<Label>("label_createdDate");

        viewModel = ViewModelManager.Instance.UniverseEditViewModel;

        charactersListController = new CharactersListViewController();
        charactersListController.Initialize(
            root,
            characterItemTemplate,
            viewModel.DeleteCharacter
        );
        charactersListController.SetItem(viewModel.Characters);


        characterCreationController = new CharacterCreationController();
        characterCreationController.Initialize(root);

        createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");

        backButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowCreateUniverse(); };

        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnDisable()
    {
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        print($"property changed: {e.PropertyName}");
        if (e.PropertyName == nameof(viewModel.CreatedDate))
        {
            createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");
        }
        else if (e.PropertyName == nameof(viewModel.Characters))
        {
            charactersListController.SetItem(viewModel.Characters);
        }
    }

    private void OnAddCharacterClicked()
    {
    }
}