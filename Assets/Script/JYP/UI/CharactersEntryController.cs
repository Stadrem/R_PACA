using System;
using UnityEngine.UIElements;

public class CharactersEntryController
{
    public interface ICharacterEntry
    {
    }

    public class CharacterEntry : ICharacterEntry
    {
        public string name;

        public string description;
        // public Texture2D image;
    }

    public class AddCharacterEntry : ICharacterEntry
    {
    }

    private VisualElement root;
    private VisualTreeAsset characterItemTemplate;
    private VisualTreeAsset addItemTemplate;

    public void Initialize(VisualElement root, VisualTreeAsset characterItemTemplate,
        VisualTreeAsset addItemTemplate)
    {
        this.root = root;
        this.characterItemTemplate = characterItemTemplate;
        this.addItemTemplate = addItemTemplate;
    }

    public void BindItem(ICharacterEntry item, Action onAddCharacterClicked)
    {
        if (item is CharacterEntry character)
        {
            root.Clear();
            var characterItem = characterItemTemplate.CloneTree();
            characterItem.Q<Label>("lbl_name").text = character.name;
            characterItem.Q<Label>("lbl_description").text = character.description;
            root.RegisterCallback<ClickEvent>(e => { });
            root.Add(characterItem);
        }
        else if (item is AddCharacterEntry)
        {
            root.Clear();
            var addItem = addItemTemplate.CloneTree();
            root.RegisterCallback<ClickEvent>(e => { onAddCharacterClicked(); });
            root.Add(addItem);
        }
    }
}