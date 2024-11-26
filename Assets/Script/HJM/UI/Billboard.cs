using UnityEngine;
using Photon.Pun;

public class Billboard : MonoBehaviourPun
{
    void LateUpdate()
    {
        // 현재 활성화된 카메라 가져오기
        Camera currentCamera = Camera.main;

        if (currentCamera != null)
        {
            // 항상 카메라를 바라보도록 회전
            transform.LookAt(transform.position + currentCamera.transform.rotation * Vector3.forward,
                             currentCamera.transform.rotation * Vector3.up);
        }
    }
}
