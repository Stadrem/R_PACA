using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSwitch : MonoBehaviour
{
    public TMP_InputField[] inputFields; // 전환할 InputField 리스트

    public Button enterBtn;

    void Update()
    {
        // Tab 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            enterBtn.onClick.Invoke();
        }
    }

    void SwitchInputField()
    {
        // 현재 활성화된 UI 요소 가져오기
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null)
        {
            // 활성화된 InputField가 리스트에 포함되어 있는지 확인
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (currentSelected == inputFields[i].gameObject)
                {
                    // 다음 InputField로 포커스 전환 (순환)
                    int nextIndex = (i + 1) % inputFields.Length;
                    inputFields[nextIndex].Select();
                    return;
                }
            }
        }

        // 기본적으로 첫 번째 InputField로 포커스
        if (inputFields.Length > 0)
        {
            inputFields[0].Select();
        }
    }

    public void OnClickSelfDisable()
    {
        gameObject.SetActive(false);
    }

    public void OnClickEnable()
    {
        gameObject.SetActive(true);
    }
}