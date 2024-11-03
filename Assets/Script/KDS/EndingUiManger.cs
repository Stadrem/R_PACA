using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingUiManger : MonoBehaviour
{
    public TMP_Text text_title;
    public TMP_Text text_theme;
    public TMP_Text text_user;
    public TMP_Text text_background;
    public TMP_Text text_ch;
    public TMP_Text text_survivor;
    public TMP_Text text_defeatMonster;
    public TMP_Text text_time;
    public TMP_Text text_goalA;
    public TMP_Text text_goalB;

    public void OnClickExitLobby()
    {
        PhotonNetwork.LeaveRoom();

        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void InputText(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
    {
        text_title.text = a;
        text_theme.text = b;
        text_user.text = c;
        text_background.text = d;
        text_ch.text = e;
        text_survivor.text = f;
        text_defeatMonster.text = g;
        text_time.text = h;
        text_goalA.text = i;
        text_goalB.text = j;
    }
}
