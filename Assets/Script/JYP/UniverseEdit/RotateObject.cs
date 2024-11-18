using System;
using UnityEngine;
using ViewModels;

namespace UniverseEdit
{
    public class RotateObject : MonoBehaviour
    {
        public float rotationAngle = 45f; // 회전 각도

        public Action onRotate;

        void Update()
        {
            // Q 키를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateY(-rotationAngle);
            }

            // E 키를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateY(rotationAngle);
            }
        }

        void RotateY(float angle)
        {
            transform.Rotate(0, angle, 0, Space.World); // Y축 기준으로 회전
            onRotate?.Invoke();
        }
    }
}