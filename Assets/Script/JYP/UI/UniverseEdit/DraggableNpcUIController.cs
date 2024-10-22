using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableNpcUIController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text npcNameText;
    public TMP_Text npcShapeText;
    public RectTransform containerRectTransform;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Init(string npcName, string npcShape)
    {
        npcNameText.text = npcName;
        npcShapeText.text = npcShape;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;

        // Make the UI element not block raycasts when dragging
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerRectTransform,
            Input.mousePosition,
            eventData.pressEventCamera,
            out var localPoint
        );
        print($"{localPoint} / Contains: {containerRectTransform.rect.Contains(localPoint)}");

        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup == null) return;

        // Re-enable raycasts when dragging ends
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = originalPosition;

        // Convert the mouse position into local point in the scrollview's RectTransform space
        Vector2 localMousePosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                containerRectTransform,
                Input.mousePosition,
                eventData.pressEventCamera,
                out localMousePosition
            ))
        {
            
        }
    }
}