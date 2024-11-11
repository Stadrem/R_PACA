using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using ViewModels;

namespace UI.Universe.Edit
{
    public class CharacterCreationController
    {
        private VisualElement root;

        [CanBeNull]
        private MonoBehaviour context = null;

        private Button selectShapeButton;
        private TextField nameInput;
        private TextField descriptionInput;
        private IntegerField hitPointsInput;
        private IntegerField strengthInput;
        private IntegerField dexterityInput;
        private Button addButton;

        // popup
        private TemplateContainer popup;
        private VisualTreeAsset selectionItemTemplate;
        private ScrollView selectionListScrollView;
        private Button popupConfirmButton;
        
        private VisualElement goblinSelectionItem;
        private VisualElement humanSelectionItem;
        private VisualElement elfSelectionItem;
        private VisualElement golemSelectionItem;
        
        private bool isPopupOpen = false;

        private NpcInfo.ENpcType selectedShapeType = NpcInfo.ENpcType.None;

        //
        private UniverseEditViewModel viewModel;


        public void Initialize(VisualElement root, MonoBehaviour contextScript)
        {
            viewModel = ViewModelManager.Instance.UniverseEditViewModel;
            context = contextScript;
            this.root = root;

            selectShapeButton = root.Q<Button>("button_selectShapeType");
            nameInput = root.Q<TextField>("input_name");
            descriptionInput = root.Q<TextField>("input_description");
            hitPointsInput = root.Q<IntegerField>("input_hp");
            strengthInput = root.Q<IntegerField>("input_strength");
            dexterityInput = root.Q<IntegerField>("input_dex");
            addButton = root.Q<Button>("button_AddCharacter");
            popup = root.Q<TemplateContainer>("selection_popup");
            popupConfirmButton = popup.Q<Button>("button_selectionConfirm");
            goblinSelectionItem = popup.Q<VisualElement>("selection_goblin");
            humanSelectionItem = popup.Q<VisualElement>("selection_human");
            elfSelectionItem = popup.Q<VisualElement>("selection_elf");
            golemSelectionItem = popup.Q<VisualElement>("selection_golem");

            goblinSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"goblin selected");
                    selectShapeButton.text = "외형: 고블린";
                    selectedShapeType = NpcInfo.ENpcType.Goblin;
                    goblinSelectionItem.AddToClassList("character-shape--selected");
                    humanSelectionItem.RemoveFromClassList("character-shape--selected");
                }
            );

            humanSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"human selected");
                    selectShapeButton.text = "외형: 인간";
                    selectedShapeType = NpcInfo.ENpcType.Human;
                    humanSelectionItem.AddToClassList("character-shape--selected");
                    goblinSelectionItem.RemoveFromClassList("character-shape--selected");
                }
            );
            
            elfSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"elf selected");
                    selectShapeButton.text = "외형: 엘프";
                    selectedShapeType = NpcInfo.ENpcType.Elf;
                    elfSelectionItem.AddToClassList("character-shape--selected");
                });
            
            golemSelectionItem.RegisterCallback<ClickEvent>(
                e =>
                {
                    Debug.Log($"golem selected");
                    selectShapeButton.text = "외형: 골렘";
                    selectedShapeType = NpcInfo.ENpcType.Golem;
                    golemSelectionItem.AddToClassList("character-shape--selected");
                });

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
            if (selectedShapeType == NpcInfo.ENpcType.None) return;


            var character = new CharacterInfo
            {
                shapeType = selectedShapeType,
                name = nameInput.value,
                description = descriptionInput.value,
                hitPoints = hitPointsInput.value,
                strength = strengthInput.value,
                dexterity = dexterityInput.value,
                isPlayable = false
            };

            Debug.Log($"hp: {character.hitPoints}, str: {character.strength}, dex: {character.dexterity}");
            context?.StartCoroutine(
                viewModel.CreateCharacter(
                    character,
                    result =>
                    {
                        if (result.IsSuccess)
                        {
                            ClearInputs();
                            CloseSelectPopup();
                        }
                        else
                        {
                            // todo: show error message
                        }
                    }
                )
            );
        }

        private void ClearInputs()
        {
            nameInput.value = "";
            descriptionInput.value = "";
            hitPointsInput.value = 0;
            strengthInput.value = 0;
            dexterityInput.value = 0;
            selectShapeButton.text = "외형 선택...";
            selectedShapeType = NpcInfo.ENpcType.None;
        }
    }
}