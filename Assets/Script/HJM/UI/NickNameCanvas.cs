using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NickNameCanvas : MonoBehaviourPunCallbacks
{
    public TMP_Text nicknameText; // 닉네임을 표시할 텍스트 컴포넌트

    private void Start()
    {
        SetNickname();
    }

    // Photon NickName을 설정하고 Custom Properties에 저장하는 함수
    public void SetNickname()
    {
        // PhotonNetwork.NickName을 설정
        PhotonNetwork.NickName = UserCodeMgr.Instance.Nickname;

        // Custom Properties에 "Nickname"을 설정해 다른 플레이어들도 읽을 수 있게 함
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Nickname", PhotonNetwork.NickName }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        UpdateNicknameText();
    }

    // 자신의 TMP_Text에 닉네임을 표시
    public void UpdateNicknameText()
    {
        if (nicknameText != null)
        {
            nicknameText.text = PhotonNetwork.NickName; // 자신의 닉네임 표시
        }
        else
        {
            Debug.LogError("nicknameText가 할당되지 않았습니다.");
        }
    }

    // 다른 플레이어들의 닉네임을 받는 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Nickname"))
        {
            // 닉네임을 다른 플레이어가 볼 수 있게 업데이트
            string updatedNickname = (string)changedProps["Nickname"];
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                nicknameText.text = updatedNickname; // 로컬 플레이어 닉네임 업데이트
            }
            else
            {
                // 다른 플레이어의 닉네임을 보여주는 추가 로직 작성
                Debug.Log($"Player {targetPlayer.ActorNumber} 닉네임: {updatedNickname}");
                // UI로 표시하는 로직 추가 가능
            }
        }
    }
}
