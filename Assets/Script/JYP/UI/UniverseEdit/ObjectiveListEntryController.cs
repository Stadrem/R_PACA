using System;
using UnityEngine.UIElements;

public class ObjectiveListEntryController
{
    private VisualElement root;
    private TextField objectiveInput;
    private VisualElement addImage;

    public void SetVisualElement(VisualElement visualElement)
    {
        objectiveInput = visualElement.Q<TextField>("input_objective");
        addImage = visualElement.Q<VisualElement>("image_add");
        root = visualElement;
    }

    public void BindData(ObjectiveListViewController.IObjectiveListEntry objective, Action addItem)
    {
        switch (objective)
        {
            case ObjectiveListViewController.ObjectiveListEntry entry:
                root.RegisterCallback<ClickEvent>(e => { } );
                objectiveInput.SetValueWithoutNotify(entry.objective);
                objectiveInput.style.display = DisplayStyle.Flex;
                addImage.style.display = DisplayStyle.None;
                break;
            case ObjectiveListViewController.AddObjectiveListEntry:
                root.RegisterCallback<ClickEvent>(e => addItem());
                objectiveInput.SetValueWithoutNotify("");
                objectiveInput.style.display = DisplayStyle.None;
                addImage.style.display = DisplayStyle.Flex;
                break;
        }
    }
}