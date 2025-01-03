﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomNPCInteraction : MonoBehaviourPunCallbacks
{
    // UI 오브젝트
    public GameObject npcUI;

    // 메인 카메라
    public Camera mainCamera;

    void Start()
    {
        // 카메라가 할당되지 않았다면, 메인 카메라를 자동으로 할당한다.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 시작할 때 UI는 비활성화
        npcUI.SetActive(false);
    }

    void Update()
    {
        // 좌클릭을 감지한다면
        if (Input.GetMouseButtonDown(0))
        {
            // 카메라에서 Raycast 발사
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast가 충돌한 경우 처리
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 오브젝트의 태그가  RoomNPC인지 확인
                if (hit.collider.CompareTag("RoomNPC"))
                {
                    // UI를 활성화
                    npcUI.SetActive(true);
                    // 게임 서버에 머물면서 로비 정보를 가져오기 위해 로비에 접속
                    PhotonNetwork.JoinLobby();
                }

                // 로비에 접속하면 호출됨

            }
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 성공적으로 접속했습니다.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"서버에서 연결이 끊겼습니다. 원인: {cause}");
    }
}
