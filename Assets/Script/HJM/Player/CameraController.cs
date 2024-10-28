﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8.0f;
    public Vector3 offset;

    public bool isBlocked = false;

    void LateUpdate()
    {
        if (isBlocked) return;
        if (target == null) return;
        
        Vector3 camPos = new Vector3(target.position.x, target.position.y + offset.y, target.position.z + offset.z);
        transform.position = camPos;

        // 카메라가 항상 타겟을 바라보게 설정
        transform.LookAt(target);


    }
}
