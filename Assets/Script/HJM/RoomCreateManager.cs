using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreateManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputRoomName;
    public TMP_InputField inputMaxPlayer;
    public Button btnCreate;

    private string roomName;
    private byte maxPlayers;

    void Start()
    {
        // inputRoomName 의 내용이 변경될 때 호출되는 함수 등록
        inputRoomName.onValueChanged.AddListener(OnValueChangedRoomName);
        // inputMaxPlayer 의 내용이 변경될 때 호출되는 함수 등록
        inputMaxPlayer.onValueChanged.AddListener(OnValueChangedMaxPlayer);
    }


    
    #region 방이름과 최대인원의 값이 있을 때만 방생성 버튼 활성화
    void OnValueChangedRoomName(string roomName)
    {
        btnCreate.interactable = roomName.Length > 0 && inputMaxPlayer.text.Length > 0;
    }

    void OnValueChangedMaxPlayer(string maxPlayer)
    {
        btnCreate.interactable = maxPlayer.Length > 0 && inputRoomName.text.Length > 0;
    }
    #endregion   


    // 방 생성 버튼을 눌렀을 때 호출 되는 함수
    public void OnClickCreateRoom()
    {
        roomName = inputRoomName.text;
        maxPlayers = byte.Parse(inputMaxPlayer.text);

        // 현재 게임서버(Room)에 있는 경우 방에서 나와 마스터 서버로 돌아가야 함
        // 로비룸은 게임서버임
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            CreateRoom();
        }
    }

    // 방에서 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("로비룸에서 나왔습니다. 마스터 서버로 이동 중...");
        PhotonNetwork.JoinLobby();  // 마스터 서버로 이동
    }

    // 마스터 서버에 연결되면 방을 생성
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버에 연결되었습니다. 방을 생성합니다.");
        CreateRoom();
    }

    // 옵션으로 방을 생성
    void CreateRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        // RoomOptions options = new RoomOptions();
        // options.MaxPlayers = maxPlayers;

        if (!PhotonNetwork.CreateRoom(roomName, options))
        {
            Debug.LogError("방 생성에 실패했습니다.");
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("방이 성공적으로 생성되었습니다.");


        // 대기실 씬으로 전환
        // 추후에 룸 생성 -> 캐릭터 선택창 -> 아바타 갱신 -> 씬 변경해야함 일단 바로 전환
        PhotonNetwork.LoadLevel("WaitingScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("방 생성 실패: " + message);
    }
}
