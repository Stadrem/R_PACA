using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarSettingPhoton : MonoBehaviourPun
{
    PlayerAvatarSetting pas;
    //PhotonView photonView; // 부모의 PhotonView 참조

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
        }
        else
        {
        }
    }

    [PunRPC]
    public void SetUserCode(int value)
    {
        // 다른 클라이언트의 userCode 값 설정
        pas.myAvatar.userCode = value;

        // userCode 설정 후, 아바타 정보 불러오기 시작
        pas.StartPostAvatarInfo(value);
    }
}
