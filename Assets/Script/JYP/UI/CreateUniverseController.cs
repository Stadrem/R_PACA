using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateUniverseController : MonoBehaviour
{
    public VisualTreeAsset objectiveItemTemplate;

    private TextField titleInput;
    private TextField genreInput;
    private TextField contentInput;
    private Label charactersLabel;
    private Label backgroundsLabel;
    private TextField tagsInput;
    private ListView objectivesListView;
    private ObjectiveListViewController objectivesListController;
    private CharactersListViewController charactersListController;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        titleInput = root.Q<TextField>("input_title");
        genreInput = root.Q<TextField>("input_genre");
        contentInput = root.Q<TextField>("input_content");
        charactersLabel = root.Q<Label>("lbl_characters");
        backgroundsLabel = root.Q<Label>("lbl_backgrounds");
        tagsInput = root.Q<TextField>("input_tags");
        objectivesListController = new ObjectiveListViewController();
        objectivesListController.Initialize(root, objectiveItemTemplate);
    }
}