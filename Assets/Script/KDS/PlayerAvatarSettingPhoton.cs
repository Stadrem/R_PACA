using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAvatarSettingPhoton : MonoBehaviourPun
{
    PlayerAvatarSetting pas;
    //PhotonView photonView; // 부모의 PhotonView 참조

    TMP_Text titleText;

    int titleIndex;

    private void Awake()
    {
        pas = GetComponentInChildren<PlayerAvatarSetting>();
    }

    void Start()
    {
        // 부모 오브젝트에서 PhotonView를 시도해서 얻어오고, 없으면 경고 메시지 출력
        if (photonView.IsMine)
        {
            if (UserCodeMgr.Instance != null)
            {
                int userCode = UserCodeMgr.Instance.UserCode;
                photonView.RPC("SetUserCode", RpcTarget.AllBuffered, userCode);
            }
            else
            {
                Debug.LogWarning("UserCodeMgr.Instance가 null입니당.");
            }

            // 방장인지 확인 후, 방장일 경우 SetOwnerIcon을 실행
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SetOwnerIcon", RpcTarget.AllBuffered);
            }

            titleText = pas.titleText;
        }
    }

    [PunRPC]
    public void SetUserCode(int value)
    {
        // 다른 클라이언트의 userCode 값 설정
        pas.myAvatar.userCode = value;

        // userCode 설정 후, 아바타 정보 불러오기 시작
        pas.RefreshAvatar();
    }

    [PunRPC]
    public void SetOwnerIcon()
    {
        pas.ShowOwnerCrown();
    }

    private void OnEnable()
    {
        // 이벤트 구독
        if (AchievementManager.Get() != null)
        {
            AchievementManager.Get().OnAchievementChanged += HandleAchievementChanged;
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (AchievementManager.Get() != null)
        {
            AchievementManager.Get().OnAchievementChanged -= HandleAchievementChanged;
        }
    }

    // 업적 변경 감지 시 호출되는 함수
    private void HandleAchievementChanged(string title, int index)
    {
        if (photonView.IsMine) // 로컬 플레이어인 경우에만 RPC 호출
        {
            photonView.RPC("UpdatePlayerTitle", RpcTarget.All, title, index);
        }
    }

    [PunRPC]
    public void UpdatePlayerTitle(string title, int index)
    {
        titleText.text = title;

        titleIndex = index;
    }
}
