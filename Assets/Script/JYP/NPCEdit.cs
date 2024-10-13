using UnityEngine;

public class NPCEdit : MonoBehaviour, IDraggable
{
    
    
    public void Init(NPCData data)
    {
    }
    public bool IsDraggable { get; } = true;

    public void StartDrag()
    {
    }

    public void Dragging(Vector3 position)
    {
        transform.position = position;
    }

    public void StopDrag()
    {
    }
}