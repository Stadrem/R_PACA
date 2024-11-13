using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarDragRotate : MonoBehaviour
{
    // 회전 속도 조절 변수
    public float rotationSpeed = 0.1f;

    // 이전 프레임의 마우스 X 위치
    private float lastMouseX;

    private void Update()
    {
        // 마우스 클릭 시작 시점
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 X 위치 저장
            lastMouseX = Input.mousePosition.x; 
        }
        // 마우스 드래그 중
        else if (Input.GetMouseButton(0))
        {
            // 현재 마우스 X 위치
            float currentMouseX = Input.mousePosition.x;

            // 마우스 이동 거리 계산
            float deltaX = currentMouseX - lastMouseX;

            // 현재 위치를 저장하여 다음 프레임에 사용
            lastMouseX = currentMouseX;

            // 캐릭터의 Y축 기준으로 회전값 조정
            transform.Rotate(0, deltaX * rotationSpeed, 0);
        }
    }
}