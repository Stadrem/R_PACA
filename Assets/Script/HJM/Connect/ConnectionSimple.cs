using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ConnectionSimple : MonoBehaviourPunCallbacks
{
    public TMP_InputField nickname;
    public Button connect;

    private const string roomName = "battleTestroom";

    void Start()
    {
        connect.onClick.AddListener(OnConnectButtonClicked);

        GenerateUserInfo();
    }

    void OnConnectButtonClicked()
    {
        if (!PhotonNetwork.IsConnected)
        {
            string userNickname = UserCodeMgr.Instance.Nickname;
            PhotonNetwork.NickName = userNickname;

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("마스터서버 접속");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("로비 접속");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        print("방 참여: " + roomName);

        PhotonNetwork.LoadLevel("Dungeon_copy");


    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("방 참여 실패: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("방 생성 실패: " + message);
    }

   
    private void GenerateUserInfo()
    {
        string userId = "User_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
        string nickname = "Player_" + Random.Range(100, 999);
        int userCode = Random.Range(100000, 999999);
        int title = Random.Range(1, 100);

        UserCodeMgr.Instance.SetUserInfo(userId, nickname);
        UserCodeMgr.Instance.SetUserCode(userCode);
        UserCodeMgr.Instance.title = title;

        print($"자동생성값 저장 - UserID: {userId}, Nickname: {nickname}, UserCode: {userCode}, Title: {title}");
    }
}
