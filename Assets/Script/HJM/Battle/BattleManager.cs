using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    [Header("플레이어 관련 목록")]
    public List<GameObject> players = new List<GameObject>();  // 플레이어 목록
    public List<UserStats> playerStats = new List<UserStats>();  // 플레이어 스탯 정보 목록
    public List<Transform> battlePos;                          // 전투 시 이동 위치
    public List<NavMeshAgent> agents = new List<NavMeshAgent>();
    public List<PlayerMove> playerMoves = new List<PlayerMove>();

    public GameObject battleUI;
    public GameObject profileUI;

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

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerGameObject in playerGameObjects)
        {
            if (!players.Contains(playerGameObject))
            {
                players.Add(playerGameObject);

                var playerStatsComponent = playerGameObject.GetComponent<UserStats>();
                if (playerStatsComponent != null)
                {
                    playerStats.Add(playerStatsComponent);
                }

                var agent = playerGameObject.GetComponent<NavMeshAgent>();
                if (agent != null) agents.Add(agent);

                var playerMove = playerGameObject.GetComponent<PlayerMove>();
                if (playerMove != null) playerMoves.Add(playerMove);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            photonView.RPC("OnBattleStart", RpcTarget.All);
        }
    }

    [PunRPC]
    void OnBattleStart()
    {
        
        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
        }
        photonView.RPC("ProfileSet", RpcTarget.All);
        battleUI.SetActive(true);
    }

    [PunRPC]
    void ProfileSet()
    {
        if (players.Count > 0)
        {
            Vector3 startPosition = profileUI.transform.position;

            for (int i = 0; i < players.Count; i++)
            {
                GameObject profile = Instantiate(profileUI, startPosition, Quaternion.identity); // PhotonNetwork.Instantiate 제거
                profile.transform.SetParent(battleUI.transform, false);
                startPosition.x += 400;
            }
        }
    }

    [PunRPC]
    void MoveToBattlePos(int playerIndex)
    {
        if (playerIndex < players.Count && playerIndex < battlePos.Count)
        {
            agents[playerIndex].enabled = false;
            playerMoves[playerIndex].clickMovementEnabled = false;
            players[playerIndex].transform.position = battlePos[playerIndex].position;
            players[playerIndex].transform.rotation = battlePos[playerIndex].rotation;
        }
    }
}
