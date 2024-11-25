using Tutorials;
using UnityEngine;

public class CreateUniverseTutorialCharacters : CreateUniverseTutorialState
{
    [SerializeField]
    private CreateUniverseTutorialStateManager manager;

    [SerializeField]
    private InfoPanelController infoPanelController;

    [SerializeField]
    private RectTransform characterSettingPanel;
    
    [SerializeField]
    private string[] infoTexts;
    
    private enum EState
    {
        None = -1,
        CharacterSetting,
        CharacterList,
        End
    }

    private EState state = EState.None;
    
    public override void OnStartState()
    {
        state = EState.CharacterSetting;
        ShowNext();
    }

    public override void OnEndState()
    {
    }
    
    private void ShowNext()
    {
        switch (state)
        {
            case EState.CharacterSetting:
                infoPanelController.SetText(infoTexts[0]);
                infoPanelController.MoveTo(1440, 0);
                state = EState.CharacterList;
                break;
            case EState.CharacterList:
                infoPanelController.SetText(infoTexts[1]);
                state = EState.End;
                break;
            case EState.End:
                
                break;
        }
    }
}