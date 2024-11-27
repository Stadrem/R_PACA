using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ViewModels;

public class CreateUniverseController : MonoBehaviour
{
    private TextField titleInput;
    private VisualElement genreFantasySelector;
    private Button charactersSettingButton;
    private TextField descriptionInput;
    private Button backgroundSettingButton;
    private Button objectiveSettingButton;
    private Button objectiveSettingButtonClicked;
    private Button backButton;

    public Action OnCharacterSettingButtonClicked = null;

    private Button saveButton;

    // private TextField tagsInput;
    private EGenreType selectedGenre = EGenreType.None;
    // private Label createdDate;


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
        descriptionInput = root.Q<TextField>("input_universe");
        charactersSettingButton = root.Q<Button>("button_characters");
        backgroundSettingButton = root.Q<Button>("button_backgrounds");
        objectiveSettingButton = root.Q<Button>("button_objective");
        objectiveSettingButtonClicked = root.Q<Button>("button_objective-clicked");
        backButton = root.Q<Button>("Button_close");
        saveButton = root.Q<Button>("Button_save");
        // tagsInput = root.Q<TextField>("input_tags");
        // createdDate = root.Q<Label>("label_createdDate");
        var popup = root.Q<TemplateContainer>("ObjectiveSelectionPopup");

        //put data from viewModel
        titleInput.value = ViewModel.Title;
        // tagsInput.value = string.Join(",", ViewModel.Tags);
        descriptionInput.value = ViewModel.Content;
        if (Enum.TryParse(ViewModel.Genre, out EGenreType genre))
        {
            selectedGenre = genre;
            genreFantasySelector.AddToClassList("character-shape--selected");
        }


        titleInput.RegisterValueChangedCallback(
            e => { ViewModel.Title = e.newValue; }
        );
        // tagsInput.RegisterValueChangedCallback(
        //     e => { ViewModel.Tags = e.newValue.Split(',').ToList(); }
        // );
        descriptionInput.RegisterValueChangedCallback(
            e => { ViewModel.Content = e.newValue; }
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

        charactersSettingButton.clicked += () =>
        {
            UniverseEditUIFlowManager.Instance.ShowCharactersEdit();
            OnCharacterSettingButtonClicked?.Invoke();
        };
        backgroundSettingButton.clicked += () => { UniverseEditUIFlowManager.Instance.ShowBackgroundEdit(); };
        objectiveSettingButton.clicked += () =>
        {
            UniverseEditUIFlowManager.Instance.ShowObjectiveSelection(
                () =>
                {
                    objectiveSettingButton.style.display = DisplayStyle.Flex;
                    objectiveSettingButtonClicked.style.display = DisplayStyle.None;
                });
            //disable button
            objectiveSettingButton.style.display = DisplayStyle.None;
            objectiveSettingButtonClicked.style.display = DisplayStyle.Flex;
        };
        backButton.clicked += () =>
        {
            ViewModelManager.Instance.Reset();
            SceneManager.LoadScene("LobbyScene");
        };
        saveButton.clicked += () =>
        {
            StartCoroutine(
                ViewModel.CreateUniverse(
                    (res) =>
                    {
                        if (res.IsSuccess)
                        {
                            ViewModelManager.Instance.Reset();
                            SceneManager.LoadScene("LobbyScene");
                        }
                        else
                            Debug.LogError($"error: {res.error}");
                    }
                )
            );
        };
        // createdDate.text = ViewModel.CreatedDate.ToString("dd/MM/yyyy");
    }
}