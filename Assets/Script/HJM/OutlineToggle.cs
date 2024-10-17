using UnityEngine;
using UnityEngine.UI;

public class OutlineToggle : MonoBehaviour
{
    public Image targetImage;       // 테두리를 추가할 이미지
    private Outline outline;        // 테두리를 위한 Outline 컴포넌트

    void Start()
    {
        // targetImage에 Outline 컴포넌트가 없다면 추가
        outline = targetImage.GetComponent<Outline>();
        if (outline == null)
        {
            outline = targetImage.gameObject.AddComponent<Outline>();
        }

        // 테두리를 비활성화된 상태로 시작
        outline.enabled = false;

        // 테두리 두께 설정 (원하는 두께로 변경 가능)
        outline.effectDistance = new Vector2(8, 8); // X, Y축으로 테두리 두께 설정
        outline.effectColor = Color.yellow;        // 테두리 색상을 노란색으로 설정 (원하는 색상으로 변경 가능)
    }

    // 이 함수는 UI 버튼 클릭 시 호출될 예정
    public void OnClickOutline()
    {
        // 테두리 활성화/비활성화 토글
        outline.enabled = !outline.enabled;
    }
}