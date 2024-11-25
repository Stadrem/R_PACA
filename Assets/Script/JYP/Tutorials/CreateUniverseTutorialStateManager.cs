using System.Linq;
using Data.Local;
using Tutorials;
using UnityEngine;
using UnityEngine.UI;

public sealed class CreateUniverseTutorialStateManager : MonoBehaviour
{
    [SerializeField]
    private CreateUniverseTutorialState[] tutorialStates;

    [SerializeField]
    private Toggle tutorialOffToggle;

    private CreateUniverseTutorialState currentState;

    private int currentTutorialStateIndex = 0;

    public void Next()
    {
        var next = tutorialStates[currentTutorialStateIndex + 1];
        TransitState(next);
    }

    private void Start()
    {
#if UNITY_EDITOR
        //for test
        PlayerPreferencesManager.IsCreateUniverseTutorialNeed = true;
#endif
        if (!PlayerPreferencesManager.IsCreateUniverseTutorialNeed)
        {
            gameObject.SetActive(false);
        }
        else
        {
            tutorialOffToggle.isOn = false;
            tutorialOffToggle.gameObject.SetActive(true);
            tutorialOffToggle.onValueChanged.AddListener(
                isOn =>
                {
                    PlayerPreferencesManager.IsCreateUniverseTutorialNeed = !isOn;
                    tutorialOffToggle.onValueChanged.RemoveAllListeners();
                    Destroy(gameObject);
                }
            );
            TransitState(tutorialStates.First());
        }
    }


    private void TransitState(CreateUniverseTutorialState state)
    {
        currentState?.OnEndState();
        currentState = state;
        currentState.OnStartState();
    }
}