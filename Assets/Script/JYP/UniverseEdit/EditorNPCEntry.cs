using UnityEngine;
using UnityEngine.EventSystems;

public class EditorNPCEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public NPCData characterData;
    public NPCSpawner npcSpawner; // Reference to NPCSpawner

    public void OnBeginDrag(PointerEventData eventData)
    {
        npcSpawner.StartDrag(gameObject); // Notify spawner we're starting to drag this entry
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the NPC entry with the mouse (this only applies to UI, 3D is handled by the spawner)
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        characterData.Position = transform.localPosition;
        npcSpawner.EndDrag(eventData); // Notify spawner the drag ended
    }
}