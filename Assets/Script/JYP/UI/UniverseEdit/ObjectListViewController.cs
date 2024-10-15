using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class ObjectiveListViewController
{
    public interface IObjectiveListEntry
    {
    }

    public struct ObjectiveListEntry : IObjectiveListEntry
    {
        public string objective;
    }

    public struct AddObjectiveListEntry : IObjectiveListEntry
    {
    }

    private VisualTreeAsset itemTemplate;
    private ListView objectiveListView;
    private readonly List<IObjectiveListEntry> objectives = new List<IObjectiveListEntry>();

    public List<string> Objectives => objectives
        .Where(entry => entry is ObjectiveListEntry)
        .Select(entry => ((ObjectiveListEntry)entry).objective)
        .ToList();

    public void Initialize(VisualElement root, VisualTreeAsset objectiveItemTemplate)
    {
        EnumerateAllObjectives();

        // Store a reference to the template for the list entries
        itemTemplate = objectiveItemTemplate;

        // Store a reference to the character list element
        objectiveListView = root.Q<ListView>("list_objectives");
        
        FillObjectivesList();

        // Register to get a callback when an item is selected
        // objectiveListView.onSelectionChange += OnObjectiveselected;
    }

    private void EnumerateAllObjectives()
    {
        objectives.Add(new ObjectiveListEntry { objective = "" });
        objectives.Add(new AddObjectiveListEntry());
    }

    void FillObjectivesList()
    {
        // Set up a make item function for a list entry
        objectiveListView.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = itemTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new ObjectiveListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        objectiveListView.bindItem = (item, index) =>
        {
            var objective = objectives[index];
            (item.userData as ObjectiveListEntryController)?.BindData(objective, AddObjective);
        };

        // Set a fixed item height
        objectiveListView.fixedItemHeight = 320;

        // Set the actual item's source list/array
        objectiveListView.itemsSource = objectives;
    }

    private void AddObjective()
    {
        objectives[^1] = new ObjectiveListEntry { objective = "" };
        objectives.Add(new AddObjectiveListEntry());
        objectiveListView.Rebuild();
    }
    //
    // void OnObjectiveselected(IEnumerable<object> selectedItems)
    // {
    //     // Get the currently selected item directly from the ListView
    //     var selectedCharacter = m_CharacterList.selectedItem as CharacterData;
    //
    //     // Handle none-selection (Escape to deselect everything)
    //     if (selectedCharacter == null)
    //     {
    //         // Clear
    //         m_CharClassLabel.text = "";
    //         m_CharNameLabel.text = "";
    //         m_CharPortrait.style.backgroundImage = null;
    //
    //         // Disable the select button
    //         m_SelectCharButton.SetEnabled(false);
    //
    //         return;
    //     }
    //
    //     // Fill in character details
    //     m_CharClassLabel.text = selectedCharacter.m_Class.ToString();
    //     m_CharNameLabel.text = selectedCharacter.m_CharacterName;
    //     m_CharPortrait.style.backgroundImage = new StyleBackground(selectedCharacter.m_PortraitImage);
    //
    //     // Enable the select button
    //     m_SelectCharButton.SetEnabled(true);
    // }
}