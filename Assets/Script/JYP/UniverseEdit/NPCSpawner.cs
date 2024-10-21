using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCSpawner : MonoBehaviour
{
    public Camera mainCamera; // Main camera for raycasting mouse position
    public GameObject npcPrefab; // Prefab to spawn the NPC
    public RectTransform npcListUI; // UI container for the NPC list
    public Transform npcParent; // Parent for the NPCs in the 3D world
    public GameObject npcEntryUIPrefab; // Prefab for the NPC entry in the UI list

    private UniverseEditViewModel viewModel;
    private GameObject draggedNPCEntry = null;
    private bool isDragging = false;
    private NpcData draggingNPCData = null;
    private int drraggedBackgroundPartId = -1;

    private void Start()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.Characters))
        {
            // Clear the NPC list UI
            foreach (Transform child in npcListUI)
            {
                Destroy(child.gameObject);
            }

            // Create a new NPC entry for each character in the view model
            foreach (var character in viewModel.Characters)
            {
                CreateNPCEntryUI(character);
            }
        }
    }

    void Update()
    {
        if (isDragging && draggedNPCEntry != null)
        {
            Vector3 mousePos = Input.mousePosition;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Move the dragged NPC entry to the mouse position in world space
                Ray ray = mainCamera.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    draggedNPCEntry.transform.position = hit.point;
                }
            }
        }
    }
    
    

    // Call this when NPC Entry begins dragging
    public void StartDrag(GameObject npcEntry)
    {
        draggedNPCEntry = npcEntry;
        draggingNPCData = npcEntry.GetComponent<EditorNPCEntry>().characterData;
        isDragging = true;
    }

    // Call this when NPC Entry is dropped
    public void EndDrag(PointerEventData eventData)
    {
        if (isDragging && draggedNPCEntry != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // If dropped outside UI, spawn the NPC in the 3D world
                var spawnedPos = SpawnNPCAtMousePosition();
                if (!spawnedPos.HasValue) return;
                // viewModel.RemoveCharacter(draggingNPCData);
                var newData = new NpcData()
                {
                    Name = draggingNPCData.Name,
                    Description = draggingNPCData.Description,
                    BackgroundPartId = drraggedBackgroundPartId,
                    Position = spawnedPos.Value
                };
                
                // viewModel.AddCharacter(newData);
                Destroy(draggedNPCEntry); // Remove NPC entry from UI
            }
            else
            {
                // CreateNPCEntryUI(draggingNPCData);
                draggingNPCData = null;
                Destroy(draggedNPCEntry); // Remove the dragged instance
            }

            draggedNPCEntry = null;
            isDragging = false;
        }
    }

    private Vector3? SpawnNPCAtMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Instantiate(npcPrefab, hit.point, Quaternion.identity, npcParent);
        }
        return hit.point;
    }

    public void TurnOffSpawnable()
    {
        npcListUI.gameObject.SetActive(false);
    }
    
    public void TurnOnSpawnable(Transform detail, int backgroundPartId)
    {
        npcListUI.gameObject.SetActive(true);
        npcListUI.position = detail.position;
        drraggedBackgroundPartId = backgroundPartId;
    }
    


    private void CreateNPCEntryUI(CharacterInfo characterInfo)
    {
        // if(!characterInfo.isPlayable)
        // {
        //         ICharacterData newData = new BaseCharacterData()
        //         {
        //             Name = characterInfo.Name,
        //             Description = characterInfo.Description,
        //         };
        //         viewModel.DeleteCharacter(characterInfo);
        //         viewModel.AddCharacter(newData);
        // }

        // Instantiate a new NPC entry UI and add it to the NPC list
        GameObject newNPCEntry = Instantiate(npcEntryUIPrefab, npcListUI);
        newNPCEntry.transform.SetParent(npcListUI, false);

        // Optionally, you can set properties or reference data on the new NPC entry here
    }
}