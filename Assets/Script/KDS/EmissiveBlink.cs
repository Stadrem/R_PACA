using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveBlink : MonoBehaviour
{
    MeshRenderer targetRenderer;

    public Material mat;

    // 기본 Emission 색상
    public Color emissionColor = Color.white;

    // Emission 변화에 걸리는 시간
    float emissionDuration = 5f; 

    // Emission 강도
    float emissionIntensity = 1f;

    // Emission 증가 방향
    bool isIncreasing = true; 

    void Start()
    {
        targetRenderer = GetComponent<MeshRenderer>();
        mat = targetRenderer.material;
    }

    private void Update()
    {
        if (mat != null)
        {
            // 0~1 사이의 값을 Time.deltaTime을 이용해 전환
            float speed = Time.deltaTime / emissionDuration;

            if (isIncreasing)
            {
                emissionIntensity += speed;
                if (emissionIntensity >= 6f)
                {
                    emissionIntensity = 6f;
                    // 방향 반전
                    isIncreasing = false; 
                }
            }
            else
            {
                emissionIntensity -= speed;
                if (emissionIntensity <= 0f)
                {
                    emissionIntensity = 0f;
                    // 방향 반전
                    isIncreasing = true; 
                }
            }

            // Emission 강도와 색상 적용
            mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
    }
}
