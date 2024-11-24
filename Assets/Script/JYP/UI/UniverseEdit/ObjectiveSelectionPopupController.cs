using System;
using UnityEngine;
using UnityEngine.UIElements;
using ViewModels;

public class ObjectiveSelectionPopupController: MonoBehaviour
{
    private const string ObjectiveSelectionHided = "objective-selection--hided";
    private const string SelectionElementSelected = "character-shape--selected";
    private VisualElement root;

    private Button confirm;
    private VisualElement selectionDemonKing;
    private VisualElement selectionCollectSword;

    private UniverseEditViewModel ViewModel => ViewModelManager.Instance.UniverseEditViewModel;

    private string[] objectiveStrings = new string[]
    {
        "성검 획득",
        "마왕 처치"
    };



    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        Hide();
        selectionCollectSword = root.Q<VisualElement>("selection_getSword");
        selectionDemonKing = root.Q<VisualElement>("selection_objectiveDemonKing");
        confirm = root.Q<Button>("button_objectiveConfirm");

        // selectionCollectSword.Q<Label>().text = objectiveStrings[0];
        // selectionDemonKing.Q<Label>().text = objectiveStrings[1];

        selectionCollectSword.RegisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.CollectSword));
        selectionDemonKing.RegisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.DefeatDemonKing));
        confirm.clicked += Hide; 
    }

    

    private void OnDisable()
    {
        selectionCollectSword.UnregisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.CollectSword));
        selectionDemonKing.UnregisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.DefeatDemonKing));
        confirm.clicked -= Hide;
    }

    public void Hide()
    {
        root?.AddToClassList(ObjectiveSelectionHided);
    }

    public void Show()
    {
        root.RemoveFromClassList(ObjectiveSelectionHided);
    }

    public void OnSelectionChanged(EObjectiveType type)
    {

        selectionCollectSword.RemoveFromClassList(SelectionElementSelected);
        selectionDemonKing.RemoveFromClassList(SelectionElementSelected);
        switch (type)
        {
            case EObjectiveType.CollectSword:
                ViewModel.Objective = objectiveStrings[0];
                selectionCollectSword.AddToClassList(SelectionElementSelected);
                break;
            case EObjectiveType.DefeatDemonKing:
                ViewModel.Objective = objectiveStrings[1];
                selectionDemonKing.AddToClassList(SelectionElementSelected);
                break;
        }
    }
}