using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InGamePlayerManager : MonoBehaviour
{
    
    
    private Vector3 spawnPos;
    private List<GameObject> playerList = new List<GameObject>();

    [SerializeField]
    private List<TestUser> userList = new List<TestUser>();
    
    

    public List<TestUser> UserList => userList;
    public TestUser myInfo;
    public TestUser MyInfo => myInfo;

    public void SetSpawnPos(Vector3 pos)
    {
        spawnPos = pos;
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate("Player_Avatar", spawnPos, Quaternion.identity);
    }

    public void ShowPlayersHpBar()
    {
        
    }
}