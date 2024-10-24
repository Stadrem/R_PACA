using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public RectTransform root;
    public RectTransform npcListTransform; // UI container for the NPC list
    public GameObject[] npcPrefabs;
    public GameObject npcUIEntryPrefab;
    private UniverseEditViewModel viewModel;

    private List<DraggableNpcUIController> npcEntries = new List<DraggableNpcUIController>();
    private Dictionary<CharacterInfo, GameObject> spawnedNpcs = new Dictionary<CharacterInfo, GameObject>();
    private int currentBackgroundPartId = -1;
    private Transform npcPositionOffset;

    private void Start()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }


    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.Characters))
        {
            ClearNpcList();
            foreach (var character in viewModel.Characters)
            {
                var entry = Instantiate(npcUIEntryPrefab, npcListTransform)
                    .GetComponent<DraggableNpcUIController>();
                entry.Init(character, SpawnNpc);
                npcEntries.Add(entry);
            }

            // find spawned npcs that are not in the view model and remove them
            var toRemove = spawnedNpcs.Keys.Where(c => !viewModel.Characters.Contains(c))
                .ToList();
            foreach (var character in toRemove)
            {
                Destroy(spawnedNpcs[character]);
                spawnedNpcs.Remove(character);
            }
        }
    }

    public void Init()
    {
        ClearNpcList();
        root.gameObject.SetActive(false);
    }

    private void ClearNpcList()
    {
        foreach (var entry in npcEntries)
        {
            Destroy(entry.gameObject);
        }

        npcEntries.Clear();

        foreach (var npc in spawnedNpcs)
        {
            Destroy(npc.Value);
        }

        spawnedNpcs.Clear();
    }

    public void StartSpawner(Transform npcOffset)
    {
        npcPositionOffset = npcOffset;
        root.gameObject.SetActive(true);
    }

    public void FinishSpawner()
    {
        root.gameObject.SetActive(false);
    }

    public void SpawnNpc(CharacterInfo character, Vector3 position)
    {
        //TODO: Spawn By it's type
        
        var npc = Instantiate(npcPrefabs[(int)character.shapeType], position, Quaternion.identity);
        npc.transform.SetParent(npcPositionOffset);
        var script = npc.GetComponent<SpawnedNpc>();
        script.characterId = character.id;
        script.npcSpawner = this;
        spawnedNpcs.Add(character, npc);
        npcEntries.Remove(npcEntries.FirstOrDefault(e => e.CharacterInfo.id == character.id));
    }

    public void ReturnToUi(int characterId)
    {
        var character = viewModel.Characters.FirstOrDefault(c => c.id == characterId);
        if (spawnedNpcs[character] == null) return;

        Destroy(spawnedNpcs[character]);
        spawnedNpcs.Remove(character);

        var entry = Instantiate(npcUIEntryPrefab, npcListTransform)
            .GetComponent<DraggableNpcUIController>();
        entry.Init(character, SpawnNpc);
        npcEntries.Add(entry);
    }
}