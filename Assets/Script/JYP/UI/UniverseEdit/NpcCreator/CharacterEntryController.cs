using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace UI.Universe.Edit
{
    public class CharacterEntryController
    {
        private VisualElement root;

        private Label shapeTypeLabel;
        private Label nameLabel;
        private Label descriptionLabel;
        private Label hpLabel;
        private Label strengthLabel;
        private Label dexLabel;
        private Button deleteButton;


        public void Initialize(VisualElement root)
        {
            this.root = root;
        }

        public void BindItem(CharacterInfo item, Action<int> onDeleteButtonClicked)
        {
            shapeTypeLabel = root.Q<Label>("lbl_shapeType");
            nameLabel = root.Q<Label>("lbl_name");
            descriptionLabel = root.Q<Label>("lbl_description");
            hpLabel = root.Q<Label>("lbl_characterHp");
            strengthLabel = root.Q<Label>("lbl_characterStrength");
            dexLabel = root.Q<Label>("lbl_characterDex");
            deleteButton = root.Q<Button>("button_RemoveCharacter");

            shapeTypeLabel.text = item.shapeType.ToString();
            nameLabel.text = item.name;
            descriptionLabel.text = item.description;
            hpLabel.text = item.hitPoints.ToString();
            strengthLabel.text = item.strength.ToString();
            dexLabel.text = item.dexterity.ToString();
            deleteButton.clicked += () => onDeleteButtonClicked(item.id);
        }
    }
}