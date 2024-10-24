using System;
using UnityEngine;
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
    private TextField tagsInput;
    private EGenreType selectedGenre = EGenreType.None;

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
        backButton = root.Q<Button>("btn_back");
        tagsInput = root.Q<TextField>("input_tags");

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
        objectiveSettingButton.clicked += () => { Debug.Log($"objective 설정 버튼 클릭!"); };
        backButton.clicked += () => { Debug.Log($"뒤로 가기(씬 나가기)"); };
        
    }
}