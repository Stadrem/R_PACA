using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class CharactersListViewController
{
    private VisualElement root;
    private VisualTreeAsset characterItemTemplate;
    private VisualTreeAsset addItemTemplate;
    private Action onAddCharacterClicked;
    private ScrollView scrollView;

    private readonly List<CharactersEntryController.ICharacterEntry> characters =
        new List<CharactersEntryController.ICharacterEntry>();

    public List<CharactersEntryController.CharacterEntry> Characters => characters
        .OfType<CharactersEntryController.CharacterEntry>()
        .ToList();


    public void Initialize(VisualElement root, VisualTreeAsset characterItemTemplate, VisualTreeAsset addItemTemplate,
        Action onAddCharacterClicked)
    {
        this.root = root;
        this.characterItemTemplate = characterItemTemplate;
        this.addItemTemplate = addItemTemplate;
        this.onAddCharacterClicked = onAddCharacterClicked;

        scrollView = root.Q<ScrollView>("scroll_characters");
    }

    public void SetItem(List<CharactersEntryController.CharacterEntry> newCharacters)
    {
        this.characters.Clear();
        this.characters.AddRange(newCharacters);
        this.characters.Add(new CharactersEntryController.AddCharacterEntry());

        OnListChanged();
    }

    private void OnListChanged()
    {
        scrollView.Clear();
        foreach (var character in characters)
        {
            CreateCharacterView(character);
        }
    }

    private void CreateCharacterView(CharactersEntryController.ICharacterEntry character)
    {
        var item = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Column,
                height = Length.Percent(100), // 각 아이템의 높이를 ScrollView에 맞춤
                width = 312
            }
        };
        var characterEntryController = new CharactersEntryController();
        characterEntryController.Initialize(item, characterItemTemplate, addItemTemplate);
        characterEntryController.BindItem(character, onAddCharacterClicked);

        scrollView.Add(item);
    }
}