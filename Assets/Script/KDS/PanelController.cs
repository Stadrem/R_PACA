using UnityEngine;
using UnityEngine.UI;


public class PanelController : MonoBehaviour
{
    public GameObject subPanel;  // SubPanel�� ����
    public VerticalLayoutGroup parentLayoutGroup;  // Mid ������Ʈ�� �ִ� Vertical Layout Group�� ����

    // �г� Ŭ�� �� ȣ��Ǵ� �Լ�
    public void ToggleSubPanel()
    {
        // ���̾ƿ� ������ �����Ͽ� ���̸� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroup.GetComponent<RectTransform>());
    }
}