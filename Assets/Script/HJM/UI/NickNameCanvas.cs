using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NickNameCanvas : MonoBehaviourPunCallbacks
{
    public TMP_Text nicknameText;
    string playerNickname;

    private void Start()
    {
        SetNickname();
    }

    public void SetNickname()
    {
        nicknameText.text = photonView.Owner.NickName;
    }

}
