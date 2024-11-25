using UnityEngine;
using Photon.Pun;

public class Billboard : MonoBehaviourPun
{
    // 카메라
    public GameObject cam;

    void Start()
    {
    }

    public void SetCamInput(GameObject gm)
    {
        cam = gm;
    }

    void LateUpdate()
    {
     
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
