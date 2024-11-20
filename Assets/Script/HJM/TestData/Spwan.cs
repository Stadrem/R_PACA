using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spwan : MonoBehaviour
{
    public Transform spwanPos;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            GameObject player = PhotonNetwork.Instantiate(
                "Player_Test", spwanPos.position, Quaternion.identity);

            UserStats userStats = player.GetComponent<UserStats>();

            if (userStats != null)
            {
                userStats.userNickname = UserCodeMgr.Instance.Nickname;
                userStats.userHealth = Random.Range(10, 16);
                userStats.userStrength = Random.Range(10, 16);
                userStats.userDexterity = Random.Range(10, 16);
            }
        }
    }
}
