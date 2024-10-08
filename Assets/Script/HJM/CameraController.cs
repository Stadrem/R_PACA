using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8.0f;
    public Vector3 offset;

    

    void Update()
    {
        if (target == null) return;

        Vector3 desirePos = new Vector3(target.position.x, target.position.y + offset.y, target.position.z + offset.z);
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desirePos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }
}
