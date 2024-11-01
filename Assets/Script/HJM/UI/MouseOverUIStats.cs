using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject DescriptionUI;
    public Vector3 offset = new Vector3(-180f, -15f, 0f); // 설명창이랑 커서가 떨어진 거리

    private bool isPointerOver = false; // 마우스가 스탯 아이콘 위에 있는지

    private void Start()
    {
        DescriptionUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        DescriptionUI.SetActive(true);
        UpdateTooltipPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        DescriptionUI.SetActive(false);
    }

    private void Update()
    {
        if (isPointerOver)
        {
            UpdateTooltipPosition();
        }
    }

    private void UpdateTooltipPosition()
    {
        Vector3 mousePosition = Input.mousePosition + offset;
        DescriptionUI.transform.position = mousePosition;
    }
}
