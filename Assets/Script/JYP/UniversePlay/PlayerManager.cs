using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 spawnPos;
    private List<GameObject> playerList = new List<GameObject>();

    public void SetSpawnPos(Vector3 pos)
    {
        spawnPos = pos;
    }
    
    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
    
}