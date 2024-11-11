using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UI.Universe.Edit
{
    public class CharactersListViewController
    {
        private VisualElement root;
        private VisualTreeAsset characterEntryTemplate;
        private ScrollView characterListScrollView;

        private Action<int> onDeleteCharacterClicked;

        public void Initialize(VisualElement root,
            VisualTreeAsset characterItemTemplate,
            Action<int> onDeleteCharacterClicked)
        {
            this.root = root;
            this.characterEntryTemplate = characterItemTemplate;
            this.onDeleteCharacterClicked = onDeleteCharacterClicked;
            characterListScrollView = root.Q<ScrollView>("scroll_characters");
        }

        public void SetItem(List<CharacterInfo> newCharacters)
        {
            characterListScrollView.Clear();
            foreach (var character in newCharacters)
            {
                CreateCharacterView(character);
                CreateSpace();
            }
        }

        private void CreateSpace()
        {
            var space = new VisualElement();
            space.style.width = 20;
            characterListScrollView.Add(space);
        }

        private void CreateCharacterView(CharacterInfo character)
        {
            var characterItem = characterEntryTemplate.CloneTree();
            
            characterItem.style.height = Length.Percent(100);
            characterItem.style.width = 480;


            var characterEntryController = new CharacterEntryController();
            characterEntryController.Initialize(characterItem);
            characterEntryController.BindItem(character, onDeleteCharacterClicked);
            characterListScrollView.Add(characterItem);
        }
    }
}