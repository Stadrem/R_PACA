using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomMgr : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputRoomName;
    public TMP_InputField inputMaxPlayer;
    public Button btnJoin;

    private string roomName;
    private byte maxPlayers;

    // 방 참여 버튼 클릭 시 호출되는 함수
    public void OnClickJoinRoom()
    {
        roomName = inputRoomName.text;
        JoinRoom(roomName); // 방에 참여하는 함수 호출
    }

    // 방에 참여하는 함수
    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName); // Photon API를 사용하여 방에 참여
    }

    // 방 참여 요청에 대한 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방에 성공적으로 참여했습니다: " + roomName);
        PhotonNetwork.LoadLevel("WaitingScene"); // 대기실 씬으로 전환
    }

    // 방 참여 실패 시 호출되는 콜백 함수
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 참여 실패: " + message);
    }
}