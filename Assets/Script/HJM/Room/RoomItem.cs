using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;

    private string roomName;
    private byte maxPlayers;

    // 안내창 프리팹과 부모 캔버스에 대한 참조
    public GameObject confirmationPopupPrefab;
    private GameObject confirmationPopupInstance;

    // 방 정보를 설정하는 함수
    public void SetRoomInfo(string roomName, int playerCount, int maxPlayers)
    {
        roomNameText.text = roomName;
        playerCountText.text = playerCount + " / " + maxPlayers;
        this.roomName = roomName; // roomName 저장
    }

    // 방 참여 버튼 클릭 시 호출되는 함수
    public void OnClickJoinRoom()
    {
        ShowConfirmationPopup(); // 확인 팝업 표시
    }

    // 참여 확인 팝업을 보여주는 함수
    private void ShowConfirmationPopup()
    {
        // 안내창 인스턴스를 생성하고 부모 캔버스에 추가
        confirmationPopupInstance = Instantiate(confirmationPopupPrefab);
        confirmationPopupInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        // 버튼에 이벤트 핸들러 연결
        var confirmButton = confirmationPopupInstance.transform.Find("Btn_YesJoin").GetComponent<Button>();
        confirmButton.onClick.AddListener(OnConfirmJoin);

        // 팝업을 활성화
        confirmationPopupInstance.SetActive(true);
    }

    // 예 버튼 클릭 시 방에 참여
    public void OnConfirmJoin()
    {
        JoinRoom(roomName);
        Destroy(confirmationPopupInstance); // 팝업 제거
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
