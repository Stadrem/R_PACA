using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NickNameCanvas : MonoBehaviourPunCallbacks
{
    public TMP_Text nicknameText;
    public Material nicknameMat1;
    string playerNickname;
    

    private void Start()
    {
        if (SceneManager.GetActiveScene()
                .name
            == "WaitingScene")
        {
            nicknameText.enableAutoSizing = false;
            nicknameText.fontSize = 24;
            nicknameText.fontMaterial = nicknameMat1;
        }
        
        SetNickname();
    }

    public void SetNickname()
    {
        nicknameText.text = photonView.Owner.NickName;
    }
}