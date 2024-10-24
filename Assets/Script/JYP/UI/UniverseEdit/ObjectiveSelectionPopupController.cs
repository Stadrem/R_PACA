using UnityEngine.UIElements;

public class ObjectiveSelectionPopupController
{
    private const string ObjectiveSelectionHided = "objective-selection--hided";
    private const string SelectionElementSelected = "character-shape--selected";
    private VisualElement root;

    private Button confirm;
    private VisualElement selectionDemonKing;
    private VisualElement selectionCollectSword;

    private EObjectiveType selectedObjectiveType = EObjectiveType.None;

    public void Init(VisualElement root)
    {
        selectionCollectSword = root.Q<VisualElement>("selection_getSword");
        selectionDemonKing = root.Q<VisualElement>("selection_objectiveDemonKing");
        confirm = root.Q<Button>("button_objectiveConfirm");

        selectionCollectSword.RegisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.CollectSword));
        selectionDemonKing.RegisterCallback<ClickEvent>(e => OnSelectionChanged(EObjectiveType.DefeatDemonKing));
        confirm.clicked += () => { Hide(); };
    }

    public void Hide()
    {
        root.AddToClassList(ObjectiveSelectionHided);
    }

    public void Show()
    {
        root.RemoveFromClassList(ObjectiveSelectionHided);
    }

    public void OnSelectionChanged(EObjectiveType type)
    {
        selectedObjectiveType = type;
        selectionCollectSword.RemoveFromClassList(SelectionElementSelected);
        selectionDemonKing.RemoveFromClassList(SelectionElementSelected);
        switch (type)
        {
            case EObjectiveType.CollectSword:
                selectionCollectSword.AddToClassList(SelectionElementSelected);
                break;
            case EObjectiveType.DefeatDemonKing:
                selectionDemonKing.AddToClassList(SelectionElementSelected);
                break;
        }
    }
}