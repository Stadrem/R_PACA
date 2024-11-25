using System.Linq;
using Data.Local;
using Tutorials;
using UnityEngine;

public sealed class CreateUniverseTutorialStateManager : MonoBehaviour
{
    [SerializeField]
    private CreateUniverseTutorialState[] tutorialStates;
    
    private CreateUniverseTutorialState currentState;

    private int currentTutorialStateIndex = 0;

    public void Next()
    {
        var next = tutorialStates[currentTutorialStateIndex + 1];
        TransitState(next);
    }

    private void Start()
    {
        if (!PlayerPreferencesManager.IsCreateUniverseTutorialNeed)
        {
            TransitState(tutorialStates.Last());
        }
    }

    private void TransitState(CreateUniverseTutorialState state)
    {
        currentState?.OnEndState();
        currentState = state;
        currentState.OnStartState();
    }
}