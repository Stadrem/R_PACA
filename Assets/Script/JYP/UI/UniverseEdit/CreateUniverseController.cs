using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ViewModels;

public class CreateUniverseController : MonoBehaviour
{
    public VisualTreeAsset objectiveItemTemplate;

    private TextField titleInput;
    private VisualElement genreFantasySelector;
    private Button charactersSettingButton;
    private Button backgroundSettingButton;
    private Button objectiveSettingButton;
    private Button backButton;
    private Button saveButton;
    private TextField tagsInput;
    private EGenreType selectedGenre = EGenreType.None;
    private Label createdDate;

    private ObjectiveSelectionPopupController objectiveSelectionPopupController;

    private static UniverseEditViewModel ViewModel => ViewModelManager.Instance.UniverseEditViewModel;

    private void Start()
    {
        ViewModel.Init();
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        titleInput = root.Q<TextField>("input_title");
        genreFantasySelector = root.Q<VisualElement>("selection_genreFantasy");
        charactersSettingButton = root.Q<Button>("button_characters");
        backgroundSettingButton = root.Q<Button>("button_backgrounds");
        objectiveSettingButton = root.Q<Button>("button_objective");
        backButton = root.Q<Button>("button_close");
        saveButton = root.Q<Button>("button_save");
        tagsInput = root.Q<TextField>("input_tags");
        createdDate = root.Q<Label>("label_createdDate");
        var popup = root.Q<TemplateContainer>("selectionPopup");
        objectiveSelectionPopupController = new ObjectiveSelectionPopupController();
        objectiveSelectionPopupController.Init(popup);

        titleInput.RegisterValueChangedCallback(
            e => { ViewModel.Title = e.newValue; }
        );


        genreFantasySelector.RegisterCallback<ClickEvent>(
            e =>
            {
                if (selectedGenre == EGenreType.Fantasy) return;
                selectedGenre = EGenreType.Fantasy;
                ViewModel.Genre = selectedGenre.ToString();
                genreFantasySelector.AddToClassList("character-shape--selected");
            }
        );

        charactersSettingButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowCharactersEdit(); };
        backgroundSettingButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowBackgroundEdit(); };
        objectiveSettingButton.clicked += () => { objectiveSelectionPopupController.Show(); };
        backButton.clicked += () => { SceneManager.LoadScene("LobbyScene"); };
        saveButton.clicked += () => { SceneManager.LoadScene("LobbyScene"); };
        createdDate.text = ViewModel.CreatedDate.ToString("dd/MM/yyyy");
    }
}