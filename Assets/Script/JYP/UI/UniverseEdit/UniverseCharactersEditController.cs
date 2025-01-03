﻿using System;
using System.ComponentModel;
using UI.Universe.Edit;
using UnityEngine;
using UnityEngine.UIElements;
using ViewModels;

public class UniverseCharactersEditController : MonoBehaviour
{
    public VisualTreeAsset characterItemTemplate;

    private UniverseEditViewModel viewModel;
    private CharactersListViewController charactersListController;
    private CharacterCreationController characterCreationController;
    private Button backButton;

    private Button backgroundButton;
    public Action OnBackgroundButtonClicked;

    private Button objectiveButton;
    private Button objectiveSettingButtonClicked;
    // private Label createdDate;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>()
            .rootVisualElement;
        backButton = root.Q<Button>("Button_Main");
        backgroundButton = root.Q<Button>("button_backgrounds");
        objectiveButton = root.Q<Button>("Button_Objectives");
        objectiveSettingButtonClicked = root.Q<Button>("button_objective-clicked");

        // createdDate = root.Q<Label>("label_createdDate");

        viewModel = ViewModelManager.Instance.UniverseEditViewModel;

        charactersListController = new CharactersListViewController();
        charactersListController.Initialize(
            root,
            characterItemTemplate,
            viewModel.DeleteCharacter
        );
        charactersListController.SetItem(viewModel.Characters);


        characterCreationController = new CharacterCreationController();
        characterCreationController.Initialize(root, this);

        // createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");

        objectiveButton.clicked += () =>
        {
            UniverseEditUIFlowManager.Instance.ShowObjectiveSelection(
                () =>
                {
                    objectiveSettingButtonClicked.style.display = DisplayStyle.None;
                    objectiveButton.style.display = DisplayStyle.Flex;
                }
            );
            objectiveButton.style.display = DisplayStyle.None;
            objectiveSettingButtonClicked.style.display = DisplayStyle.Flex;
        };
        backButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowCreateUniverse(); };
        backgroundButton.clicked += () =>
        {
            OnBackgroundButtonClicked?.Invoke();
            UniverseEditUIFlowManager.Instance.ShowBackgroundEdit();
        };
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
            // createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");
        }
        else if (e.PropertyName == nameof(viewModel.Characters))
        {
            charactersListController.SetItem(viewModel.Characters);
        }
    }
}