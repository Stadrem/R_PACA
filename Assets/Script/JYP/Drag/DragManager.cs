using UnityEngine;

public class DragManager : MonoBehaviour
{
    public bool isDraggable = true;
    private IDraggable currentDraggable;
    private bool isDragging = false;
    private Camera camera;
    public LayerMask dragPointLayer;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼을 누를 때
        if (Input.GetMouseButtonDown(0))
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
        // 마우스 왼쪽 버튼을 누르고 있을 때
        else if (Input.GetMouseButton(0))
        {
            if (!isDragging) return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, dragPointLayer))
            {
                currentDraggable.Dragging(hit.point);
            }
        }
        // 마우스 왼쪽 버튼을 뗄 때
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;

            print("Stop Drag");
            currentDraggable.StopDrag();
            isDragging = false;
        }
    }
}