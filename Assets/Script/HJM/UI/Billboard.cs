using UnityEngine;
using Photon.Pun;

public class Billboard : MonoBehaviourPun
{
    // 카메라
    public Camera cam;

    void Start()
    {
        // 카메라가 할당되지 않았다면, 메인 카메라를 자동으로 할당한다.
        //if (cam == null)
        //{
        //    cam = Camera.main;
        //}
    }


    void LateUpdate()
    {
        //if (cam == null)
        //{
        //    cam = Camera.main; // 카메라가 없으면 다시 할당
        //}

        if (cam != null)
        {
            // PhotonView로 확인하여 모든 플레이어의 오브젝트에 대해 빌보드 적용
            //if (photonView.IsMine || !photonView.IsMine)
            {
                // 항상 카메라를 바라보도록 회전
                transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                                cam.transform.rotation * Vector3.up);
            }
        }
    }
}
