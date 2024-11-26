using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmissiveBlink : MonoBehaviour
{
    MeshRenderer targetRenderer;

    public Material mat;

    // 기본 Emission 색상
    public Color emissionColor = Color.white;
    private Color initialColor;
    
    // Emission 변화에 걸리는 시간
    public float emissionDuration = 0.1f;

    public int maxEmissionPower = 24;

    // Emission 강도
    float emissionIntensity = 0.01f;

    // Emission 증가 방향
    bool isIncreasing = true; 

    void Start()
    {
        targetRenderer = GetComponent<MeshRenderer>();
        initialColor = emissionColor;
        
        if(mat == null)
        {
            mat = targetRenderer.material;
        }
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
                if (emissionIntensity >= maxEmissionPower)
                {
                    emissionIntensity = maxEmissionPower;
                    // 방향 반전
                    isIncreasing = false; 
                }
            }
            else
            {
                emissionIntensity -= speed;
                if (emissionIntensity <= 1f)
                {
                    emissionIntensity = 1f;
                    // 방향 반전
                    isIncreasing = true; 
                }
            }

            // Emission 강도와 색상 적용
            mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
        // 소멸 시 초기 색상으로 복구
        mat?.SetColor("_EmissionColor", Color.white);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        mat?.SetColor("_EmissionColor",  Color.white);
    }
}
