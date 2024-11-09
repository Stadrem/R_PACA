using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ConnectionMgr : MonoBehaviourPunCallbacks
{
    public void OnClickConnect()
    {
        Alert.Get().Set("로비 입장 시도 중");
        // 마스터 서버에 접속 시도
        Debug.Log("마스터 서버에 접속을 시도했습니다.");
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버에 접속 성공하면 호출되는 함수
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버에 접속을 성공했습니다.");
        // 로비 접속
        JoinLobby();
    }

    public void JoinLobby()
    {
        Alert.Get().Set("환영합니다.");
        // 닉네임 설정
        PhotonNetwork.NickName = UserCodeMgr.Instance.Nickname;
        // 기본 Lobby 입장
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장이 성공하면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비서버 입장 완료했습니다.");
        PhotonNetwork.LoadLevel("LobbyScene");
        //JoinOrCreatLobbyRoom();

    }

}