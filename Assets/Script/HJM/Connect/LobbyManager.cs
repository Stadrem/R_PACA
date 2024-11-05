using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Spawn 위치를 담아놓는 변수(나중에 배열로 써도됨)
    public Vector3 spawnPos;


    void Start()
    {
        // 플레이어를 생성 (현재 로비룸에 함께 접속해 있는 유저들에게도 보이게 동기화)
        PhotonNetwork.Instantiate("Player_Avatar", spawnPos, Quaternion.identity);
        // 리소스 폴더내에서 / 경로 설정하고 폴더 파도 됨

        // 유저데이터 받아와서 넣어줘야겠네,,

        // RPC 보내는 빈도 설정
        PhotonNetwork.SendRate = 60;

        // OnPhotonSerializeView 보내고 받고 하는 빈도 설정
        PhotonNetwork.SerializationRate = 60;

    }
}
