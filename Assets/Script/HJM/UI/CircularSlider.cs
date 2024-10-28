using UnityEngine;
using UnityEngine.UI;

public class CircularSlider : MonoBehaviour
{
    public Image sliderImage;          // 슬라이더의 이미지
    public float sliderValue = 1.0f;   // 초기 슬라이더 값 (1로 시작)
    public float maxValue = 1.0f;      // 슬라이더의 최대값
    public float depletionTime = 8.0f; // 슬라이더가 0이 되기까지 걸리는 시간 (8초)

    private bool isDepleting = false;  // 슬라이더 감소 여부를 확인하는 변수

    void Start()
    {
        isDepleting = true;
        UpdateSlider(sliderValue);
    }

    // 슬라이더 값을 업데이트하는 함수
    void Update()
    {
        // isDepleting이 true일 때만 슬라이더 감소
        if (isDepleting && sliderValue > 0)
        {
            DepleteSlider();
        }
    }
    public void UpdateSlider(float value)
    {
        sliderValue = Mathf.Clamp(value, 0, maxValue);  // 슬라이더 값 제한
        sliderImage.fillAmount = sliderValue / maxValue;
    }

    // 슬라이더 감소를 시작하는 함수
    public void StartDepletion()
    {
        isDepleting = true;
    }

    // 슬라이더 감소를 멈추는 함수
    public void OnClickStopSlider()
    {
        isDepleting = false;
    }

    // 시간 경과에 따라 슬라이더 값을 줄이는 함수
    private void DepleteSlider()
    {
        float depletionRate = maxValue / depletionTime;
        UpdateSlider(sliderValue - depletionRate * Time.deltaTime);
    }

}
