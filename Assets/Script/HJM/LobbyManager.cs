using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Spawn 위치를 담아놓는 변수(나중에 배열로 써도됨)
    public Vector3 spawnPos;


    void Start()
    {
        // RPC 보내는 빈도 설정
        PhotonNetwork.SendRate = 60;
        // OnPhotonSerializeView 보내고 받고 하는 빈도 설정
        PhotonNetwork.SerializationRate = 60;

        // 인스턴스 오류 뜸 순서안맞아서 그런듯 집가서 해결해라

        // 플레이어를 생성 (현재 Room 에 접속 되어있는 친구들도 보이게)
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
