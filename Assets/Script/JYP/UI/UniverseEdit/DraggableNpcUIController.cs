using System;
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

    private Action<CharacterInfo, Vector3> onDroppedInGround;
    private CharacterInfo characterInfo;
    public CharacterInfo CharacterInfo => characterInfo;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        containerRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void Init(CharacterInfo character, Action<CharacterInfo, Vector3> onDroppedOnGround)
    {
        characterInfo = character;
        npcNameText.text = character.name;
        npcShapeText.text = character.shapeType.ToString();
        onDroppedInGround = onDroppedOnGround;
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
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup == null) return;

        canvasGroup.blocksRaycasts = true;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, 1 << LayerMask.NameToLayer("DetailGround")))
        {
            onDroppedInGround?.Invoke(characterInfo, hit.point);
            Destroy(gameObject);
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}