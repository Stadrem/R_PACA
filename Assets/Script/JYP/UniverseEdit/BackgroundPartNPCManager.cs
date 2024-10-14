using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BackgroundPartNPCManager : MonoBehaviour
{
    public GameObject NPCPrefab;
    
    private UniverseEditViewModel viewModel;
    private List<GameObject> placedNPC;
    
    private void Start()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }
    

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if(propertyChangedEventArgs.PropertyName == nameof(viewModel.CharacterEntries))
        {
            
        }
    }

    public void SpawnNPC(Transform parent, int characterId)
    {
        var character = viewModel.CharacterEntries[characterId];
        var npc = Instantiate(NPCPrefab, parent);
        npc.transform.localPosition = Vector3.zero;
        placedNPC.Add(npc);
        
    }
}