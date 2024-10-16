using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; // PhotonHashTable 사용을 위한 네임스페이스

public class InfoSetUp : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // 로컬 플레이어의 유저 정보 설정
        if (photonView.IsMine)
        {
            // 유저 정보를 CustomProperties에 저장
            SetCustomUserProperties("Player_123", "PlayerName", 0, 1, 2, 1, 1);
        }

        // 동기화된 유저 정보 확인
        object userName;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("userName", out userName))
        {
            Debug.Log("유저 이름: " + (string)userName);
        }
    }

    // CustomProperties 설정
    public void SetCustomUserProperties(string userID, string userName, int gender, int hair, int body, int skin, int hand)
    {
        // 해시 테이블에 유저 정보 저장
        Hashtable playerProperties = new Hashtable();
        playerProperties["userID"] = userID;
        playerProperties["userName"] = userName;
        playerProperties["gender"] = gender;
        playerProperties["hair"] = hair;
        playerProperties["body"] = body;
        playerProperties["skin"] = skin;
        playerProperties["hand"] = hand;

        // 로컬 플레이어의 CustomProperties에 추가
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("userName"))
        {
            Debug.Log(targetPlayer.NickName + "의 유저 이름이 변경되었습니다: " + changedProps["userName"]);
        }
    }
}
