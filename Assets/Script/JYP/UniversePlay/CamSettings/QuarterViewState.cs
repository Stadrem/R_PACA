public class QuarterViewState : ICamSettingState
{
    public void OnEnter()
    {
        PlayUniverseManager.Instance.InGamePlayerManager.UnblockPlayerCamera();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        PlayUniverseManager.Instance.InGamePlayerManager.BlockPlayerCamera();
    }
}