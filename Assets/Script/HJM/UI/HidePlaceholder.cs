using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputFieldPlaceholder : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField inputField;   // TMP_InputField 컴포넌트
    private TMP_Text placeholderText; // Placeholder 텍스트 객체

    void Start()
    {
        // TMP_InputField 컴포넌트를 가져옴
        inputField = GetComponent<TMP_InputField>();

        // Placeholder 텍스트 컴포넌트를 가져옴
        if (inputField.placeholder != null)
        {
            placeholderText = inputField.placeholder.GetComponent<TMP_Text>();
        }

        // 시작 시 Placeholder 상태 설정
        ShowPlaceholder();
    }

    // 마우스 클릭 시 호출되는 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        HidePlaceholder();
    }

    // Placeholder를 숨기는 함수
    void HidePlaceholder()
    {
        if (placeholderText != null)
        {
            placeholderText.enabled = false;
        }
    }

    // Placeholder를 다시 보이게 하는 함수
    public void ShowPlaceholder()
    {
        // 입력된 텍스트가 없는 경우에만 Placeholder를 보이게 함
        if (string.IsNullOrEmpty(inputField.text))
        {
            if (placeholderText != null)
            {
                placeholderText.enabled = true;
            }
        }
    }

    void Update()
    {
        // 입력란의 내용이 변경될 때마다 Placeholder 상태를 업데이트
        if (inputField != null)
        {
            if (string.IsNullOrEmpty(inputField.text)) // 수정된 부분
            {
                ShowPlaceholder();
            }
            else
            {
                if (placeholderText != null)
                {
                    placeholderText.enabled = false; // 텍스트가 있으면 Placeholder 숨김
                }
            }
        }
    }
}
