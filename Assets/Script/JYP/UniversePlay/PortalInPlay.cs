using UnityEngine;

public class PortalInPlay : MonoBehaviour
{
    private int targetBackgroundId;

    public void Init(PortalData portalData)
    {
        this.targetBackgroundId = portalData.targetBackgroundId;
        transform.position = portalData.position;
    }

    public void InteractByUser()
    {
        PlayUniverseManager.Instance.BackgroundManager.MoveTo(targetBackgroundId);

    }
}