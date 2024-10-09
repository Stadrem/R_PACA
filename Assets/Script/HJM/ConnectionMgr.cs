using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ConnectionMgr : MonoBehaviourPunCallbacks
{
    // 버튼 Connect
    public Button btnConnect;

    public void OnClickConnect()
    {
        // 마스터 서버에 접속 시도
        Debug.Log("마스터 서버에 접속을 시도했습니다.");
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버에 접속 성공하면 호출되는 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버에 접속을 성공했습니다.");
        // 씬 전환 (지금은 테스트씬으로 이동)
        PhotonNetwork.LoadLevel("PlayerMoveScene");

    }
}
