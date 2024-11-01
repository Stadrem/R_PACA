using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarSettingPhoton : MonoBehaviourPun
{
    PlayerAvatarSetting pas;

    void Start()
    {
        pas = GetComponent<PlayerAvatarSetting>();

        // 주 클라이언트에서만 userCode 값 설정 및 동기화
        if (photonView.IsMine)
        {
            int userCode = UserCodeMgr.Instance.UserCode;

            photonView.RPC("SetUserCode", RpcTarget.AllBuffered, userCode);
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
