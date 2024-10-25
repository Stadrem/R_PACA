using UnityEngine;
using UnityEngine.UI;

public class CircularSlider : MonoBehaviour
{
    public Image sliderImage;  // 슬라이더의 이미지
    public float sliderValue = 0.5f;  // 초기 슬라이더 값 (0 ~ 1)
    public float maxValue = 1.0f;  // 슬라이더의 최대값

    private void Start()
    {
        // 초기값 설정
        UpdateSlider(sliderValue);
    }

    // 슬라이더 값을 업데이트하는 함수
    public void UpdateSlider(float value)
    {
        sliderValue = Mathf.Clamp(value, 0, maxValue);  // 슬라이더 값 제한 (0~1 사이)
        sliderImage.fillAmount = sliderValue / maxValue;  // Image의 fillAmount를 업데이트
    }

    // 테스트를 위한 함수: 키보드 입력으로 슬라이더 값을 변경
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            UpdateSlider(sliderValue + Time.deltaTime);  // 증가
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            UpdateSlider(sliderValue - Time.deltaTime);  // 감소
        }
    }
}
