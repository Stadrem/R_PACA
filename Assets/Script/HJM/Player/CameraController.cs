using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8.0f;
    public Vector3 offset;

    public bool isBlocked = false;


    void Awake()
    {
        PhotonView pv = transform.parent.GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            Billboard[] bill = GameObject.FindObjectsOfType<Billboard>();

            for (int i = 0; i < bill.Length; i++)
            {
                bill[i].cam = transform.gameObject;
            }

        }
    }
    private void Start()
    {
        // 포톤뷰 내 것 일때만 오디오리스너 활성화
        GetComponent<AudioListener>().enabled = target != null && target.GetComponent<PhotonView>().IsMine;
    }

    void LateUpdate()
    {
        if (isBlocked || target == null) return;

        Vector3 camPos = new Vector3(target.position.x, target.position.y + offset.y, target.position.z + offset.z);
        transform.position = camPos;

        // 카메라가 항상 타겟을 바라보게 설정
        transform.LookAt(target);
    }
}
