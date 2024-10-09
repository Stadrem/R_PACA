
using UnityEngine;

public interface IDraggable
{
    bool IsDraggable { get; }
    void StartDrag();
    void Dragging(Vector3 position);
    
    void StopDrag();
}