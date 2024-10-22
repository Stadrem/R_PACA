using System;
using UnityEngine;

public class SpawnedNpc : MonoBehaviour, IDraggable
{
    public bool IsDraggable => true;
    public NPCSpawner npcSpawner;
    public int characterId { get; set; } = -1;
    private Vector3 startPos = Vector3.zero;

    public void StartDrag()
    {
        startPos = transform.position;
    }

    public void Dragging(Vector3 position)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, 1 << LayerMask.NameToLayer("DetailGround")))
        {
            transform.position = position;
        }
    }

    public void StopDrag()
    {
        // check ray cast for UI or detail ground
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (RectTransformUtility.RectangleContainsScreenPoint(
                npcSpawner.npcListTransform,
                Input.mousePosition
            ))
        {
            npcSpawner.ReturnToUi(characterId);
        }
        else if (Physics.Raycast(ray, out var hit, float.MaxValue, 1 << LayerMask.NameToLayer("DetailGround")))
        {
        }
        else
        {
            transform.position = startPos;
        }
    }
}