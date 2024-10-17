using UnityEngine;

public class PortalInPlay : MonoBehaviour
{
    private int targetBackgroundId;

    private void Awake()
    {
    }

    private void Update()
    {
    }

    public void Init(PortalData portalData)
    {
        this.targetBackgroundId = portalData.targetBackgroundId;
        transform.position = portalData.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UniversePlayManager.Instance.BackgroundManager.MoveTo(targetBackgroundId);
        }
    }
}