using System;
using UI.Universe.Edit;
using UnityEngine;

public class UniverseEditUIFlowManager : MonoBehaviour
{
    [SerializeField] private CreateUniverseController createUniverseController;

    [SerializeField] private UniverseCharactersEditController universeCharactersEditController;

    [SerializeField] private BackgroundPartLinkManager backgroundPartLinkManager;
    
    [SerializeField] private BackgroundEditUIController backgroundEditUIController;

    private static UniverseEditUIFlowManager instance;

    public static UniverseEditUIFlowManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("UniverseEditUIFlowManager");
                instance = go.AddComponent<UniverseEditUIFlowManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        
        ShowCreateUniverse();
    }

    public void ShowCreateUniverse()
    {
        if (universeCharactersEditController.gameObject.activeSelf)
            universeCharactersEditController.gameObject.SetActive(false);
        if(backgroundEditUIController.gameObject.activeSelf)
            backgroundEditUIController.gameObject.SetActive(false);
        
        createUniverseController.gameObject.SetActive(true);
    }

    public void ShowCharactersEdit()
    {
        if (createUniverseController.gameObject.activeSelf)
            createUniverseController.gameObject.SetActive(false);
        
        universeCharactersEditController.gameObject.SetActive(true);
    }
    
    public void ShowBackgroundEdit()
    {
        if (createUniverseController.gameObject.activeSelf)
            createUniverseController.gameObject.SetActive(false);
     
        backgroundEditUIController.ShowUI();
    }
}