using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    [Header("플레이어 관련 목록")]
    public List<GameObject> players = new List<GameObject>();  // 플레이어 목록
    public List<UserInfo> playerStats = new List<UserInfo>();  // 플레이어 스탯 정보 목록
    public List<Transform> battlePos;                          // 전투 시 이동 위치
    public List<NavMeshAgent> agents = new List<NavMeshAgent>();
    public List<PlayerMove> playerMoves = new List<PlayerMove>();

    [Header("NPC 생성 옵션")]
    public GameObject npcPrefab;
    public Transform npcPos;
    public int npcCount = 1;
    private int currentNpcCount = 0;
    public GameObject npc;

    public GameObject battleUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializePlayers();
        if (players.Count > 0)
        {
            SortPlayersByDexterity();
        }
    }

    // 플레이어 초기화 함수
    private void InitializePlayers()
    {
        var playerObjects = GameObject.FindObjectsOfType<UserInfo>();
        foreach (var playerObj in playerObjects)
        {
            var playerGameObject = playerObj.gameObject;
            players.Add(playerGameObject);
            playerStats.Add(playerObj);

            var agent = playerGameObject.GetComponent<NavMeshAgent>();
            if (agent != null) agents.Add(agent);

            var playerMove = playerGameObject.GetComponent<PlayerMove>();
            if (playerMove != null) playerMoves.Add(playerMove);
        }
    }

    // 손재주 순으로 플레이어 정렬
    private void SortPlayersByDexterity()
    {
        playerStats = playerStats.OrderByDescending(p => p.userDexterity).ToList();
        players = playerStats.Select(p => p.gameObject).ToList();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            if (currentNpcCount < npcCount)
            {
                SpawnNPCs();
            }

            MoveToBattlePos();
            battleUI.SetActive(true);
        }
    }

    // NPC 생성
    public void SpawnNPCs()
    {
        if (currentNpcCount < npcCount)
        {
            npc = PhotonNetwork.Instantiate(npcPrefab.name, npcPos.position, npcPos.rotation);
            currentNpcCount++;
        }
    }

    // 플레이어를 전투 위치로 이동
    public void MoveToBattlePos()
    {
        for (int i = 0; i < players.Count; i++)
        {
            agents[i].enabled = false;
            playerMoves[i].clickMovementEnabled = false;
            players[i].transform.position = battlePos[i].position;
            players[i].transform.rotation = battlePos[i].rotation;
        }
    }
}
