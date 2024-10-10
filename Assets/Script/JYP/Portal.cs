using UnityEngine;

public class Portal : MonoBehaviour, IDraggable
{
    public LinkedBackgroundPart towardPart;
    public bool IsDraggable => true;

    public void StartDrag()
    {
        //do Nothing
    }

    public void Dragging(Vector3 position)
    {
        transform.position = position;
    }

    public void StopDrag()
    {
        //do Nothing
    }
}