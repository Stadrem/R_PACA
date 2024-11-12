using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;
using Data.Remote.Api;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using UniversePlay;
using ViewModels;

public class InGamePlayerManager : MonoBehaviour
{
    public class PlayerInfo
    {
        public int code;
        public string id;
        public string name;
        public int hitPoints;
        public int strength;
        public int dexterity;
    }


    private Vector3 spawnPos;
    private List<HpBarSystem> playerHpSystemList = new List<HpBarSystem>();

    public PlayerInfo CurrentPlayerInfo => playerList[0];

    public List<PlayerInfo> playerList = new List<PlayerInfo>();
    private CameraController currentPlayerCameraController;

    public int PlayerCount => playerList.Count;


    public void Init()
    {
    }

    /// <summary>
    /// 시나리오 플레이를 위해 플레이어 정보를 추가하는 함수
    /// </summary>
    /// <param name="userCode">user 식별 ID값</param>
    /// <param name="playerId">플레이어 ID string</param>
    /// <param name="payerName">플레이어 이름</param>
    /// <param name="hitPoints">플레이어 HP</param>
    /// <param name="strength">플레이어 Str</param>
    /// <param name="dexterity">플레이어 Dex</param>
    public void AddPlayer(int userCode, string playerId, string payerName, int hitPoints, int strength, int dexterity)
    {
        playerList.Add(
            new PlayerInfo
            {
                code = userCode,
                id = playerId,
                name = payerName,
                hitPoints = hitPoints,
                strength = strength,
                dexterity = dexterity
            }
        );
    }

    public void AddCurrentPlayer(int userCode, string playerId, string payerName, int hitPoints, int strength,
        int dexterity)
    {
        playerList.Insert(
            0,
            new PlayerInfo
            {
                code = userCode,
                id = playerId,
                name = payerName,
                hitPoints = hitPoints,
                strength = strength,
                dexterity = dexterity
            }
        );
    }

    /// <summary>
    /// "playerId"을 가진 플레이어의 정보를 업데이트하는 함수
    /// </summary>
    /// <param name="playerId">업데이트할 유저 이름</param>
    /// <param name="hitPoints">업데이트될 HP</param>
    /// <param name="strength">업데이트될 str</param>
    /// <param name="dexterity">업데이트될 dex</param>
    public void UpdatePlayer(string playerId, int hitPoints, int strength, int dexterity)
    {
        var playerInfo = playerList.Find(info => info.id == playerId);
        if (playerInfo == null)
        {
            Debug.LogError("해당 이름을 가진 플레이어가 없습니다.");
            return;
        }

        playerInfo.hitPoints = hitPoints;
        playerInfo.strength = strength;
        playerInfo.dexterity = dexterity;
    }

    public void UpdatePlayerHitPoint(int userCode, string playerId, int hp)
    {
        var playerInfo = playerList.Find(info => info.code == userCode);

        if (playerInfo == null)
        {
            Debug.LogError("해당 이름을 가진 플레이어가 없습니다.");
            return;
        }

        var newPlayer = new PlayerInfo
        {
            code = userCode,
            id = playerId,
            name = playerInfo.name,
            hitPoints = hp,
            strength = playerInfo.strength,
            dexterity = playerInfo.dexterity
        };

        UpdateStat(
            newPlayer,
            () => { playerInfo.hitPoints = hp; }
        );
    }

    public void UpdatePlayerStrength(int userCode, string playerId, int str)
    {
        var playerInfo = playerList.Find(info => info.code == userCode);
        if (playerInfo == null)
        {
            Debug.LogError("해당 이름을 가진 플레이어가 없습니다.");
            return;
        }

        var newPlayer = new PlayerInfo
        {
            code = userCode,
            id = playerId,
            name = playerInfo.name,
            hitPoints = playerInfo.hitPoints,
            strength = str,
            dexterity = playerInfo.dexterity
        };

        UpdateStat(
            newPlayer,
            () => { playerInfo.strength = str; }
        );
    }

    public void UpdatePlayerDexterity(int userCode, string playerId, int dex)
    {
        var playerInfo = playerList.Find(info => info.code == userCode);
        if (playerInfo == null)
        {
            Debug.LogError("해당 이름을 가진 플레이어가 없습니다.");
            return;
        }

        var newPlayer = new PlayerInfo
        {
            code = userCode,
            id = playerId,
            name = playerInfo.name,
            hitPoints = playerInfo.hitPoints,
            strength = playerInfo.strength,
            dexterity = dex
        };

        UpdateStat(
            newPlayer,
            () => { playerInfo.dexterity = dex; }
        );
    }

    public void DeletePlayer(int userCode)
    {
        var playerInfo = playerList.Find(info => info.code == userCode);
        if (playerInfo == null)
        {
            Debug.LogError("해당 이름을 가진 플레이어가 없습니다.");
            return;
        }

        playerList.Remove(playerInfo);
    }


    /// <summary>
    /// PUN이 전역적으로 호출하기 때문에 SpawnPlayers라는 이름을 가졌지만, 하나의 플레이어만 스폰한다.
    /// </summary>
    public void SpawnPlayers()
    {
        spawnPos = GameObject.Find("SpawnPoint")
                       ?.transform.position
                   ?? Vector3.zero;

        var playerInfo = playerList[0];

        SpawnPlayer(playerInfo);
        currentPlayerCameraController = Camera.main?.GetComponent<CameraController>();
    }

    public void SpawnPlayer(PlayerInfo playerInfo)
    {
        Debug.Log($"생성!!");
        var go = PhotonNetwork.Instantiate("Player_Avatar", spawnPos, Quaternion.identity);
        var hpBarSystem = go.GetComponent<HpBarSystem>();
        go.GetComponent<UserStats>().Initialize(playerInfo.hitPoints, playerInfo.strength, playerInfo.dexterity);
        hpBarSystem.Init(playerInfo.hitPoints);
        playerHpSystemList.Add(hpBarSystem);
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

    private void UpdateStat(PlayerInfo playerInfo, Action onUpdated)
    {
        var universeid = PhotonNetwork.CurrentRoom.CustomProperties[PunPropertyNames.Room.ScenarioCode];
        var universePlayerSetting = new UniversePlayerSettings(
            (int)universeid,
            playerInfo.code,
            new CharacterStats(playerInfo.hitPoints, playerInfo.strength, playerInfo.dexterity)
        );

        StartCoroutine(
            ScenarioUserSettingsApi.UploadUserSettings(
                universePlayerSetting,
                (res) =>
                {
                    if (res.IsSuccess)
                        onUpdated();
                }
            )
        );
    }
}