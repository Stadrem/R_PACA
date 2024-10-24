using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

    private UniverseEditViewModel viewModel;


    private void OnEnable()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;


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

        genreFantasySelector.RegisterCallback<ClickEvent>(
            e =>
            {
                selectedGenre = EGenreType.Fantasy;
                genreFantasySelector.AddToClassList("character-shape--selected");
            }
        );

        charactersSettingButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowCharactersEdit(); };
        backgroundSettingButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowBackgroundEdit(); };
        objectiveSettingButton.clicked += () => { objectiveSelectionPopupController.Show(); };
        backButton.clicked += () => { SceneManager.LoadScene("LobbyScene"); };
        saveButton.clicked += () => {SceneManager.LoadScene("LobbyScene"); };
        createdDate.text = viewModel.CreatedDate.ToString("dd/MM/yyyy");
    }
}