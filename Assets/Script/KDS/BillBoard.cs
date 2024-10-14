using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform camTransform;

    void LateUpdate()
    {
        camTransform = Camera.main.transform;

        transform.LookAt(transform.position + (camTransform.rotation * Vector3.forward));
    }
}
