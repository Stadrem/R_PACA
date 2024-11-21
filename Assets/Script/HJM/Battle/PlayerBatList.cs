using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerBatInfo
{
    public string nickname;
    public int health;
    public int strength;
    public int dexterity;
    public int viewID; // 추가: PhotonView.ViewID 저장

    public PlayerBatInfo(string nickname, int health, int strength, int dexterity, int viewID)
    {
        this.nickname = nickname;
        this.health = health;
        this.strength = strength;
        this.dexterity = dexterity;
        this.viewID = viewID; // PhotonView.ViewID 저장
    }
}

public class PlayerBatList : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<PlayerBatInfo> battlePlayers;

    private void Awake()
    {
        if (battlePlayers == null)
        {
            battlePlayers = new List<PlayerBatInfo>();
        }
    }

    public List<PlayerBatInfo> GetBattlePlayers()
    {
        return battlePlayers;
    }

    [PunRPC]
    public void RegisterPlayer(string nickname, int health, int strength, int dexterity, int viewID)
    {
        PlayerBatInfo newPlayer = new PlayerBatInfo(nickname, health, strength, dexterity, viewID);
        battlePlayers.Add(newPlayer);

        // 정렬: ViewID 기준으로 정렬
        battlePlayers = battlePlayers.OrderBy(player => player.viewID).ToList();

        Debug.Log("Players sorted by ViewID:");
        foreach (var player in battlePlayers)
        {
            Debug.Log($"Nickname: {player.nickname}, ViewID: {player.viewID}");
        }
    }
}
