using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JYP
{
    public class TestForPhoton : MonoBehaviourPunCallbacks
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // 마스터 서버에 접속 시도
                Debug.Log("마스터 서버에 접속을 시도했습니다.");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("마스터 서버에 접속을 성공했습니다.");
            // 로비 접속
            JoinLobby();
        }

        public void JoinLobby()
        {
            // 닉네임 설정
            PhotonNetwork.NickName = "플레이어";
            // 기본 Lobby 입장
            PhotonNetwork.JoinLobby();
        }

        // 로비 입장이 성공하면 호출되는 함수
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("로비서버 입장 완료했습니다.");
            JoinOrCreatLobbyRoom();

        }

        // Room을 참여하자. 만약에 해당 Room이 없으면 Room을 만든다.
        public void JoinOrCreatLobbyRoom()
        {
            RoomOptions lobbyRoomOption = new RoomOptions();
            lobbyRoomOption.MaxPlayers = 20; // 포톤 최대 인원
            lobbyRoomOption.IsVisible = false; // 로비로 사용할 방이라 보이지 않게 설정
            lobbyRoomOption.IsOpen = true; // 방 참여 가능 여부

            // LoobyRoom 참여(시작하자마자 자동으로 로비룸에 입장시킬 것임

            // 혹은 생성
            PhotonNetwork.JoinOrCreateRoom("LobbyRoom", lobbyRoomOption, TypedLobby.Default);
        }

        // 로비룸 생성 성공 했을 때 호출되는 함수
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("로비룸 생성 완료했습니다.");

        }

        // 로비룸에 입장 성공 했을 때 호출되는 함수
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("로비룸 입장 완료했습니다.");

            // 로비씬으로 전환
        }
    }
}