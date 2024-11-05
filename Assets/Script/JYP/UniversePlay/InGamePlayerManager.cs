using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UniversePlay;

public class InGamePlayerManager : MonoBehaviour
{
    private Vector3 spawnPos;
    private List<HpBarSystem> playerHpSystemList = new List<HpBarSystem>();
    private List<CharacterInfo> characterInfoList = new List<CharacterInfo>();

    [SerializeField] private List<TestUser> userList = new List<TestUser>();

    private CameraController currentPlayerCameraController;

    public int PlayerCount => userList.Count;

    public List<TestUser> UserList => userList;
    public TestUser myInfo;
    public TestUser MyInfo => myInfo;

    public void Init()
    {
        //todo : Get Data When Server Ready
        characterInfoList = new List<CharacterInfo>()
        {
            new()
            {
                description = "",
                hitPoints = 10,
                strength = 4,
                dexterity = 7,
                id = 1,
                name = "TestUser",
            }
        };
    }


    public void SpawnPlayers()
    {
        spawnPos = GameObject.Find("SpawnPoint")
                       ?.transform.position
                   ?? Vector3.zero;
        foreach (var user in characterInfoList)
        {
            SpawnPlayer(user);
            currentPlayerCameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        }
    }

    public void SpawnPlayer(CharacterInfo characterInfo)
    {
        var go = PhotonNetwork.Instantiate("Player_Avatar", spawnPos, Quaternion.identity);
        var hpBarSystem = go.GetComponent<HpBarSystem>();
        hpBarSystem.Init(characterInfo.hitPoints);
        playerHpSystemList.Add(hpBarSystem);
        PlayUniverseManager.Instance.NpcManager.selectorChat = go.GetComponent<ISelectorChat>();
    }

    public void ShowPlayersHpBar()
    {
        foreach (var hpBarSystem in playerHpSystemList)
        {
            hpBarSystem.ShowHpBar();
        }
    }

    public void HidePlayersHpBar()
    {
        foreach (var hpBarSystem in playerHpSystemList)
        {
            hpBarSystem.HideHpBar();
        }
    }

    public void BlockPlayerCamera()
    {
        currentPlayerCameraController.isBlocked = true;
    }

    public void UnblockPlayerCamera()
    {
        currentPlayerCameraController.isBlocked = false;
    }
}