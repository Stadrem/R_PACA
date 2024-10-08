using UnityEngine;
using UnityEngine.UI;


public class PanelController : MonoBehaviour
{
    public GameObject subPanel;  // SubPanel을 참조
    public VerticalLayoutGroup parentLayoutGroup;  // Mid 오브젝트에 있는 Vertical Layout Group을 참조

    // 패널 클릭 시 호출되는 함수
    public void ToggleSubPanel()
    {
        // 레이아웃 갱신을 강제하여 높이를 재계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
    }
}