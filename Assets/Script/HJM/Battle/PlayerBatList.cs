using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBatInfo
{
    public string nickname;
    public int health;
    public int strength;
    public int dexterity;

    public PlayerBatInfo(string nickname, int health, int strength, int dexterity)
    {
        this.nickname = nickname;
        this.health = health;
        this.strength = strength;
        this.dexterity = dexterity;
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

    // battlePlayers반환
    public List<PlayerBatInfo> GetBattlePlayers()
    {
        return battlePlayers;
    }


    private void OnValidate()
    {
        SortBattlePlayersByDexterity();
    }

    private void SortBattlePlayersByDexterity()
    {
        if (battlePlayers != null)
        {
            battlePlayers.Sort((a, b) => b.dexterity.CompareTo(a.dexterity));
        }
    }

    [PunRPC]
    public void RegisterPlayer(string nickname, int health, int strength, int dexterity)
    {
        PlayerBatInfo newPlayer = new PlayerBatInfo(nickname, health, strength, dexterity);
        battlePlayers.Add(newPlayer);
    }
}