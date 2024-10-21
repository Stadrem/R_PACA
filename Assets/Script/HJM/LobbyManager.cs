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

    [Header("방 생성 입력값")]
    [SerializeField]
    // Input Room Name
    public TMP_InputField inputRoomName;
    // Input Max Player
    public TMP_InputField inputMaxPlayer;
    // Input Password
    //public TMP_InputField inputPassword;
    // Create Button
    public Button btnCreate;
    // Join Button
    public Button btnJoin;

    // 전체 방에 대한 정보
    Dictionary<string, RoomInfo> allRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // 플레이어를 생성 (현재 로비룸에 함께 접속해 있는 유저들에게도 보이게 동기화)
        PhotonNetwork.Instantiate("Player_Avatar", spawnPos, Quaternion.identity);
        // 리소스 폴더내에서 / 경로 설정하고 폴더 파도 됨

        // RPC 보내는 빈도 설정
        PhotonNetwork.SendRate = 60;
        // OnPhotonSerializeView 보내고 받고 하는 빈도 설정
        PhotonNetwork.SerializationRate = 60;

    }
    // 로비에 있을 때 방에대한 정보들이 변경되면 호출되는 함수
    // roomList : 전체 방목록 X , 변경된 방 정보   
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        // 방목록 UI 를 전체 삭제
        RemoveRoomList();

        // 전체 방 정보를 갱신
        UpdateRoomList(roomList);

        // allRoomInfo 를 기반으로 방목록 UI 를 만들자
        CreateRoomList();

        //for(int i = 0; i < roomList.Count; i++)
        //{
        //    print(roomList[i].Name + "," + roomList[i].PlayerCount + ", " + roomList[i].RemovedFromList);
        //}
    }

    void UpdateRoomList(List<RoomInfo> roomList)
    {

        for (int i = 0; i < roomList.Count; i++)
        {
            // allRoomInfo 에 roomList 의 i 번째 정보가 있니? (roomList[i] 의 방이름이 key 값으로 존재하니?)
            if (allRoomInfo.ContainsKey(roomList[i].Name))
            {
                // allRoomInfo 수정 or 삭제
                // 삭제 된 방이니?
                if (roomList[i].RemovedFromList == true)
                {
                    allRoomInfo.Remove(roomList[i].Name);
                }
                // 수정
                else
                {
                    allRoomInfo[roomList[i].Name] = roomList[i];
                }
            }
            else
            {
                // allRoomInfo 추가
                // allRoomInfo.Add(roomList[i].Name, roomList[i]);
                if (roomList[i].RemovedFromList == false)
                {
                    allRoomInfo[roomList[i].Name] = roomList[i];
                }
            }
        }
    }

    // RoomItem 의 Prefab
    public GameObject roomItemFactory;
    // ScrollView 의 Contetn Transform
    public RectTransform trContent;
    void CreateRoomList()
    {
        foreach (RoomInfo info in allRoomInfo.Values)
        {
            // roomItem prefab 을 이용해서 roomItem 을 만든다.
            GameObject go = Instantiate(roomItemFactory, trContent);
            // 만들어진 roomItem 의 내용을 변경
            // RoomItem 컴포넌트 가져오자
            RoomItem roomItem = go.GetComponent<RoomItem>();

            // 커스텀 정보 중 방 이름 가져오자.
            string roomName = (string)info.CustomProperties["room_name"];
            // 커스텀 정보 중 잠금 모드 가져오자.
            bool isLock = (bool)info.CustomProperties["lock_mode"];
            // 커스텀 정보 중 Map 종류 가져오자
            int mapIdx = (int)info.CustomProperties["map"];

            // 가져온 컴포넌트에 정보를 입력 
            // 방이름 ( 5 / 10 )
            roomItem.SetConent(roomName, info.PlayerCount, info.MaxPlayers);
            // 잠금 모드 표현
            roomItem.SetLockMode(isLock);
            // roomItem 에 mapIdx 전달
            roomItem.SetMapIndex(mapIdx);

            // roomItem 이 클릭되었을 때 호출되는 함수 등록
            roomItem.onChangeRoomName = OnChangeRoomNameField;
        }
    }

    void RemoveRoomList()
    {
        // trContent 에 있는 자식을 모두 삭제
        for (int i = 0; i < trContent.childCount; i++)
        {
            Destroy(trContent.GetChild(i).gameObject);
        }
    }

    // Map Image 담을 변수
    public Image imgMap;
    // Map Sprite 들 담을 변수
    public Sprite[] mapThumbnails;
    void OnChangeRoomNameField(string roomName, int mapIdx)
    {
        // 방 이름 설정
        inputRoomName.text = roomName;

        
    }
}
