using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public List<Transform> seatPositions; // 자리 위치 트랜스폼 리스트


    private static UserCodeMgr UserCodeMgr => UserCodeMgr.Instance;

    // 시나리오 플레이를 위한 플레이어 매니저
    private InGamePlayerManager PlayerManager => PlayUniverseManager.Instance?.InGamePlayerManager;
    private bool currentPlayerAdded = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // get game manager
                PlayUniverseManager.Create();
            }

            CreatePlayerAvatar();
        }
    }

    private void Update()
    {
        if (PlayerManager != null && !currentPlayerAdded)
        {
            PlayerManager.AddCurrentPlayer(UserCodeMgr.UserCode, UserCodeMgr.UserID, UserCodeMgr.Nickname, 0, 0, 0);
            currentPlayerAdded = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var playerID = (string)player.CustomProperties[PunPropertyNames.Player.PlayerId];
                var playerUserCode = (int)player.CustomProperties[PunPropertyNames.Player.PlayerUserCode];
                if (playerUserCode == UserCodeMgr.UserCode)
                {
                    continue;
                }

                PlayerManager.AddPlayer(playerUserCode, playerID, player.NickName, 0, 0, 0);
            }
        }
    }

    // 플레이어 아바타 생성 메서드
    private void CreatePlayerAvatar()
    {
        // 현재 플레이어의 ID에 따라 자리 할당
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 자리의 인덱스를 기반으로 위치 설정
        Vector3 position = GetSeatPosition(playerIndex);
        print(playerIndex);
        print(position);

        Debug.Log($"생성! {position}");
        // 포톤 인스턴스 생성
        GameObject playerAvatar = PhotonNetwork.Instantiate("Player_Avatar", position, Quaternion.Euler(0, 180, 0));
        print(11111111111111);

        playerAvatar.name = "Player_Avatar_" + PhotonNetwork.NickName;
        print(11111111111111);
    }


    private Vector3 GetSeatPosition(int index)
    {
        if (index >= 0 && index < seatPositions.Count)
        {
            print(11111111111111);
            return seatPositions[index].position; // 위치 변환
        }

        return Vector3.zero;
    }

    // 플레이어가 방에 들어왔을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var playerId = (string)newPlayer.CustomProperties[PunPropertyNames.Player.PlayerId];
        var playerUserCode = (int)newPlayer.CustomProperties[PunPropertyNames.Player.PlayerUserCode];
        // 새로운 플레이어가 들어왔을 때 좌석을 조정합니다.
        AdjustSeats();
        print(11111111111111);

        PlayerManager.AddPlayer(playerUserCode, playerId, newPlayer.NickName, 0, 0, 0);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (otherPlayer.CustomProperties.TryGetValue(PunPropertyNames.Player.PlayerUserCode, out object userCode))
        {
            AdjustSeats();
            PlayerManager.DeletePlayer((int)userCode);
        }
    }

    // 좌석 조정
    private void AdjustSeats()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Player player = PhotonNetwork.CurrentRoom.Players[i + 1];
            // 각 플레이어 아바타의 위치를 설정
            Vector3 position = GetSeatPosition(i);
            print(11111111111111);
        }
    }
}