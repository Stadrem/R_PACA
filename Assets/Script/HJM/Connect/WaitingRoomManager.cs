using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public List<Transform> seatPositions; // 자리 위치 트랜스폼 리스트

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            CreatePlayerAvatar();
        }
    }

    // 플레이어 아바타 생성 메서드
    private void CreatePlayerAvatar()
    {
        // 현재 플레이어의 ID에 따라 자리 할당
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 자리의 인덱스를 기반으로 위치 설정
        Vector3 position = GetSeatPosition(playerIndex);

        // 포톤 인스턴스 생성
        GameObject playerAvatar = PhotonNetwork.Instantiate("Player_Avatar", position, Quaternion.Euler(0, 180, 0));

        // 추가적인 초기화 코드 필요 시 여기에 추가
    }

    
    private Vector3 GetSeatPosition(int index)
    {
        if (index >= 0 && index < seatPositions.Count)
        {
            return seatPositions[index].position; // 위치 변환
        }
        return Vector3.zero;
    }

    // 플레이어가 방에 들어왔을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새로운 플레이어가 들어왔을 때 좌석을 조정합니다.
        AdjustSeats();
    }

    // 좌석 조정
    private void AdjustSeats()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Player player = PhotonNetwork.CurrentRoom.Players[i + 1];
            // 각 플레이어 아바타의 위치를 설정
            Vector3 position = GetSeatPosition(i);
            
        }
    }
}
