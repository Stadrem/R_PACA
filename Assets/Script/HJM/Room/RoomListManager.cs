using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    // ScrollView의 Content가 될 Transform
    public RectTransform roomListContent;

    // RoomItem 프리팹
    public GameObject roomItemPrefab;

    //public GameObject cam;
    void Start()
    {
    }

    //public void OnClickJoinLobby()
    //{
    //    // 현재 게임서버(Room)에 있는 경우 방에서 나와 마스터 서버로 돌아가야 함
    //    // 로비룸은 게임서버임
    //    if (PhotonNetwork.InRoom)
    //    {
    //        //cam.SetActive(true);
    //        PhotonNetwork.LeaveRoom();
    //    }
    //    // 게임 서버에 머물면서 로비 정보를 가져오기 위해 로비에 접속
    //    PhotonNetwork.JoinLobby();
    //}

    //// 로비에 접속하면 호출됨
    //public override void OnJoinedLobby()
    //{
        
    //    Debug.Log("로비에 성공적으로 접속했습니다.");
    //}

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"서버에서 연결이 끊겼습니다. 원인: {cause}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        Debug.Log($"받은 방 리스트: {roomList.Count}개");

        RemoveAllRoomItems();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}");
            if (!room.RemovedFromList)
            {
                AddRoomItem(room);
            }
        }
    }



    // 방 목록 아이템을 UI에 추가하는 함수
    void AddRoomItem(RoomInfo roomInfo)
    {
        // RoomItem 프리팹을 생성하고 ScrollView의 Content에 추가
        GameObject roomItemObject = Instantiate(roomItemPrefab, roomListContent);

        // RoomItem 컴포넌트를 가져와 방 정보를 업데이트
        RoomItem roomItem = roomItemObject.GetComponent<RoomItem>();

        string roomName = roomInfo.Name;
        int playerCount = roomInfo.PlayerCount;
        int maxPlayers = roomInfo.MaxPlayers;

        // 방 정보 설정 (RoomName, 현재 인원 / 최대 인원)
        roomItem.SetRoomInfo(roomName, playerCount, maxPlayers);
    }

    // 모든 방 목록 아이템을 삭제하는 함수
    void RemoveAllRoomItems()
    {
        for (int i = 0; i < roomListContent.childCount; i++)
        {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }
}
