using ExitGames.Client.Photon;
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
    //public Button btnJoin;
    
    private string roomName;
    private byte maxPlayers;

    /// <summary>
    /// 시나리오 리스트 UI 컨트롤러, 방 생성할 때 나오는 시나리오 선택창에 대한 컨트롤러
    /// </summary>
    [SerializeField]
    private ScenarioListUIController scenarioListUIController;

    void Start()
    {
        
        // inputRoomName 의 내용이 변경될 때 호출되는 함수 등록
        inputRoomName.onValueChanged.AddListener(OnValueChangedRoomName);
        // inputMaxPlayer 의 내용이 변경될 때 호출되는 함수 등록
        inputMaxPlayer.onValueChanged.AddListener(OnValueChangedMaxPlayer);

        //btnJoin.onClick.AddListener(OnClickJoinRoom); // 방 참여 버튼 클릭 이벤트 등록
        //btnJoin.interactable = false; // 초기 상태에서 비활성화
        
        scenarioListUIController.Init();
    }

    #region 방이름과 최대인원의 값이 있을 때만 방생성 버튼 활성화

    void OnValueChangedRoomName(string roomName)
    {
        btnCreate.interactable = roomName.Length > 0 && inputMaxPlayer.text.Length > 0;
        UpdateJoinButtonInteractable(); // 방 이름이 변경될 때 방 참여 버튼 상태 업데이트
    }

    void OnValueChangedMaxPlayer(string maxPlayer)
    {
        btnCreate.interactable = maxPlayer.Length > 0 && inputRoomName.text.Length > 0;
        UpdateJoinButtonInteractable(); // 최대 인원이 변경될 때 방 참여 버튼 상태 업데이트
    }

    // 방 참여 버튼 활성화 여부 업데이트
    void UpdateJoinButtonInteractable()
    {
        //btnJoin.interactable = inputRoomName.text.Length > 0;
    }

    #endregion

    // 방 생성 버튼을 눌렀을 때 호출 되는 함수
    public void OnClickCreateRoom()
    {
        var selectedScenario = scenarioListUIController.GetSelectedScenario();
        if (selectedScenario == null)
        {
            Debug.LogError("시나리오를 선택해주세요.");
            return;
        }
        
        // 방 옵션 추가
        var option = new Hashtable
        {
            { PunPropertyNames.Room.ScenarioCode, selectedScenario.scenarioCode },
            { PunPropertyNames.Room.ScenarioTitle, selectedScenario.scenarioTitle }
        };

        roomName = inputRoomName.text;
        maxPlayers = byte.Parse(inputMaxPlayer.text);

        CreateRoom(option);
    }

    //// 방에서 나갔을 때 호출되는 콜백
    //public override void OnLeftRoom()
    //{
    //    Debug.Log("로비룸에서 나왔습니다. 마스터 서버로 이동 중...");
    //    PhotonNetwork.JoinLobby();  // 마스터 서버로 이동
    //}

    //// 마스터 서버에 연결되면 방을 생성
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("마스터 서버에 연결되었습니다. 방을 생성합니다.");
    //}

    // 옵션으로 방을 생성
    void CreateRoom(
        Hashtable customRoomProperties = null
    )
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            CustomRoomProperties = customRoomProperties,
        };
        
        
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
        PhotonNetwork.LoadLevel("WaitingScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("방 생성 실패: " + message);
    }
}