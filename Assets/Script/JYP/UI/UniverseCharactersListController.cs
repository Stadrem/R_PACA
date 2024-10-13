using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class UniverseCharactersListController : MonoBehaviour
{
    public VisualTreeAsset characterItemTemplate;
    public VisualTreeAsset addCharacterTemplate;


    private UniverseEditViewModel viewModel;
    private CharactersListViewController charactersListController;
    private Button backButton;
    private Label createdDate;


    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        charactersListController = new CharactersListViewController();
        charactersListController.Initialize(
            root,
            characterItemTemplate,
            addCharacterTemplate,
            onAddCharacterClicked: OnAddCharacterClicked
        );
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        backButton = root.Q<Button>("btn_back");
        createdDate = root.Q<Label>("label_createdDate");
        charactersListController.SetItem(viewModel.Characters);
        backButton.clicked += () => { };
        createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");

    }

    private void OnDisable()
    {
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
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
        viewModel.Characters.Add(new CharactersEntryController.CharacterEntry());
    }
}