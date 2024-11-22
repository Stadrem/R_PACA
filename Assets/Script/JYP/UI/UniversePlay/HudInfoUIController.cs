using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace UI.UniversePlay
{
    public class HudInfoUIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text userNicknameText;
        
        private void Start()
        {
            userNicknameText.text = PhotonNetwork.NickName;
        }
    }
}