using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform camTransform;

    void LateUpdate()
    {
        if (Camera.main == null) return;
        camTransform = Camera.main.transform;
        transform.LookAt(transform.position + (camTransform.rotation * Vector3.forward));
    }
}