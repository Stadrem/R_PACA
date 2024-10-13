using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class CharactersListViewController
{
    private VisualElement root;
    private VisualTreeAsset characterItemTemplate;
    private VisualTreeAsset addItemTemplate;

    private ListView listView;

    private readonly List<CharactersEntryController.ICharacterEntry> characters =
        new List<CharactersEntryController.ICharacterEntry>();

    public List<CharactersEntryController.CharacterEntry> Characters => characters
        .OfType<CharactersEntryController.CharacterEntry>()
        .ToList();


    public void Initialize(VisualElement root, VisualTreeAsset characterItemTemplate, VisualTreeAsset addItemTemplate,
        Action onAddCharacterClicked)
    {
        listView = root.Q<ListView>("list_characters");
        listView.makeItem = () =>
        {
            var item = characterItemTemplate.CloneTree();
            var controller = new CharactersEntryController();
            controller.Initialize(item, characterItemTemplate, addItemTemplate);
            item.userData = controller;
            return item;
        };
        
        listView.bindItem = (e, i) =>
        {
            var controller = e.userData as CharactersEntryController;
            controller?.BindItem(characters[i], onAddCharacterClicked: onAddCharacterClicked);
        };

        listView.itemsSource = characters;
    }

    public void SetItem(List<CharactersEntryController.CharacterEntry> newCharacters)
    {
        this.characters.Clear();
        this.characters.AddRange(newCharacters);
        this.characters.Add(new CharactersEntryController.AddCharacterEntry());
        listView.Rebuild();
    }
}