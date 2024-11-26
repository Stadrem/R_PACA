using UnityEngine;
using Photon.Pun;

public class BillboardStart : MonoBehaviourPun
{
    private float timer = 0f;
    private float duration = 0.5f;

    private void LateUpdate()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;

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
}
