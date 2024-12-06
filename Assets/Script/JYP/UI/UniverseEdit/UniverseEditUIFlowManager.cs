using System;
using UI.Universe.Edit;
using UnityEngine;
using UniverseEdit;

public class UniverseEditUIFlowManager : MonoBehaviour
{
    [SerializeField]
    private CreateUniverseController createUniverseController;

    [SerializeField]
    private UniverseCharactersEditController universeCharactersEditController;

    [SerializeField]
    private BackgroundPartLinkManager backgroundPartLinkManager;

    [SerializeField]
    private BackgroundEditUIController backgroundEditUIController;

    [SerializeField]
    private ObjectiveSelectionPopupController objectiveSelectionPopupController;

    public enum EEditUIState : byte
    {
        Main = 0b0,
        CharacterEdit = 0b01,
        BackgroundEdit = 0b10,
        ObjectiveSelection = 0b100,
    }

    private EEditUIState currentState = EEditUIState.Main;
    public EEditUIState CurrentState => currentState;

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
        if (backgroundEditUIController.gameObject.activeSelf)
            backgroundEditUIController.gameObject.SetActive(false);
        objectiveSelectionPopupController.Hide();
        createUniverseController.gameObject.SetActive(true);

        // currentState = 0b100
    }

    public void ShowCharactersEdit()
    {
        if (createUniverseController.gameObject.activeSelf)
            createUniverseController.gameObject.SetActive(false);
        if (backgroundEditUIController.gameObject.activeSelf)
            backgroundEditUIController.gameObject.SetActive(false);
        objectiveSelectionPopupController.Hide();
        universeCharactersEditController.gameObject.SetActive(true);
    }

    public void ShowBackgroundEdit()
    {
        if (createUniverseController.gameObject.activeSelf)
            createUniverseController.gameObject.SetActive(false);

        if (universeCharactersEditController.gameObject.activeSelf)
            universeCharactersEditController.gameObject.SetActive(false);
        objectiveSelectionPopupController.Hide();
        backgroundEditUIController.ShowUI();
    }

    public void ShowObjectiveSelection(Action onConfirm)
    {
        objectiveSelectionPopupController.Show(
            onConfirm
        );
    }
}