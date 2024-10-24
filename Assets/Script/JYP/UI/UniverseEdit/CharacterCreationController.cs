﻿using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Universe.Edit
{
    public class CharacterCreationController
    {
        private VisualElement root;

        private Button selectShapeButton;
        private TextField nameInput;
        private TextField descriptionInput;
        private IntegerField hitPointsInput;
        private IntegerField strengthInput;
        private IntegerField dexterityInput;
        private Toggle playableToggle;
        private Button addButton;

        // popup
        private TemplateContainer popup;
        private VisualTreeAsset selectionItemTemplate;
        private ScrollView selectionListScrollView;
        private Button popupConfirmButton;
        private VisualElement goblinSelectionItem;
        private VisualElement humanSelectionItem;
        private bool isPopupOpen = false;

        private ECharacterShapeType selectedShapeType = ECharacterShapeType.None;

        //
        private UniverseEditViewModel viewModel;


        public void Initialize(VisualElement root)
        {
            viewModel = ViewModelManager.Instance.UniverseEditViewModel;

            this.root = root;

            selectShapeButton = root.Q<Button>("button_selectShapeType");
            nameInput = root.Q<TextField>("input_name");
            descriptionInput = root.Q<TextField>("input_description");
            hitPointsInput = root.Q<IntegerField>("input_hp");
            strengthInput = root.Q<IntegerField>("input_strength");
            dexterityInput = root.Q<IntegerField>("input_dex");
            playableToggle = root.Q<Toggle>("Toggle_playable");
            addButton = root.Q<Button>("button_AddCharacter");
            popup = root.Q<TemplateContainer>("selection_popup");
            popupConfirmButton = popup.Q<Button>("button_selectionConfirm");
            goblinSelectionItem = popup.Q<VisualElement>("selection_goblin");
            humanSelectionItem = popup.Q<VisualElement>("selection_human");

            goblinSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"goblin selected");
                    selectShapeButton.text = "외형: 고블린";
                    selectedShapeType = ECharacterShapeType.Goblin;
                    goblinSelectionItem.AddToClassList("character-shape--selected");
                    humanSelectionItem.RemoveFromClassList("character-shape--selected");
                }
            );

            humanSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"human selected");
                    selectShapeButton.text = "외형: 인간";
                    selectedShapeType = ECharacterShapeType.Human;
                    humanSelectionItem.AddToClassList("character-shape--selected");
                    goblinSelectionItem.RemoveFromClassList("character-shape--selected");
                }
            );

            selectShapeButton.clicked += OpenSelectPopup;
            addButton.clicked += OnAddButtonClicked;
            popupConfirmButton.clicked += CloseSelectPopup;
        }

        private void OpenSelectPopup()
        {
            if (isPopupOpen) return;
            popup.RemoveFromClassList("shape-selection-popup--hide");
            isPopupOpen = true;
        }

        private void CloseSelectPopup()
        {
            if (!isPopupOpen) return;
            popup.AddToClassList("shape-selection-popup--hide");
            isPopupOpen = false;
        }

        private void OnAddButtonClicked()
        {
            if (selectedShapeType == ECharacterShapeType.None) return;

            var character = new CharacterInfo
            {
                shapeType = selectedShapeType,
                name = nameInput.value,
                description = descriptionInput.value,
                hitPoints = hitPointsInput.value,
                strength = strengthInput.value,
                dexterity = dexterityInput.value,
                isPlayable = playableToggle.value
            };

            viewModel.AddCharacter(character);
            ClearInputs();
            CloseSelectPopup();
        }

        private void ClearInputs()
        {
            nameInput.value = "";
            descriptionInput.value = "";
            hitPointsInput.value = 0;
            strengthInput.value = 0;
            dexterityInput.value = 0;
            playableToggle.value = false;
            
            selectShapeButton.text = "외형 선택...";
            selectedShapeType = ECharacterShapeType.None;
        }
    }
}