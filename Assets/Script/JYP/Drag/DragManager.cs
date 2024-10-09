using UnityEngine;

public class DragManager : MonoBehaviour
{
    public bool isDraggable = true;
    private IDraggable currentDraggable;
    private bool isDragging = false;
    private Camera camera;
    public LayerMask dragPointLayer = 1 << LayerMask.NameToLayer("Ground");
    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isDragging) return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                currentDraggable = hit.collider.GetComponent<IDraggable>();
                if (currentDraggable == null) return;
                if (!currentDraggable.IsDraggable) return;
                currentDraggable.StartDrag();
                isDragging = true;
            }
        }
        else if (Input.GetKey(KeyCode.G))
        {
            if (!isDragging) return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, dragPointLayer))
            {
                currentDraggable.Dragging(hit.point);
            }
        }
        else if (Input.GetKeyUp(KeyCode.G))
        {
            if (!isDragging) return;

            currentDraggable.StopDrag();
            isDragging = false;
        }
    }
}