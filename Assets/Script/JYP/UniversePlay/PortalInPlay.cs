using UnityEngine;

public class PortalInPlay : MonoBehaviour
{
    private int targetBackgroundId;

    public void Init(PortalData portalData)
    {
        this.targetBackgroundId = portalData.targetBackgroundId;
        transform.position = portalData.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayUniverseManager.Instance.BackgroundManager.MoveTo(targetBackgroundId);
        }
    }
}