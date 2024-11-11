using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    [Header("플레이어 관련 목록")]
    public List<GameObject> players = new List<GameObject>();  // 플레이어 목록
    public List<UserStats> playerStats = new List<UserStats>();  // 플레이어 스탯 정보 목록
    public List<Transform> battlePos;                          // 전투 시 이동 위치
    public List<NavMeshAgent> agents = new List<NavMeshAgent>();
    public List<PlayerMove> playerMoves = new List<PlayerMove>();

    public List<GameObject> profiles;

    public GameObject battleUI;
    public GameObject profileUI;

    //public bool isMyTurnAction = false;

    public PlayerBatList playerBatList;

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

    void Start()
    {

    }

    void Update()
    {
        //if (TurnCheckSystem.Instance.isMyTurn && isMyTurnAction == true)
        //{
        //    PerformAction();
        //}

        if (Input.GetKeyDown(KeyCode.B))
        {
            photonView.RPC("OnBattleStart", RpcTarget.All);
        }

        if (players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            InitializePlayers();
        }
    }

    //private void PerformAction()
    //{
    //    Debug.Log("턴 선택 행동 ~~");
    //    isMyTurnAction = false;
    //    Debug.Log("턴 선택 행동 끝");

    //    TurnCheckSystem.Instance.EndTurn();
    //}
    private void InitializePlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        //print("플레이어 오브젝트 수 : " + playerGameObjects.Length.ToString());
        GameObject[] Playerss = new GameObject[playerGameObjects.Length];

        List<UserStats> tempPlayerStats = new List<UserStats>();
        List<NavMeshAgent> tempAgents = new List<NavMeshAgent>();
        List<PlayerMove> tempPlayerMoves = new List<PlayerMove>();

        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            PhotonView pv = playerGameObjects[i].GetComponent<PhotonView>();
            int playerindex = pv.ViewID / 1000;
            //print(playerindex);

            // 플레이어 게임 오브젝트 리스트에 추가
            Playerss[playerindex - 1] = playerGameObjects[i];

            // 컴포넌트들 가져오기
            UserStats stats = playerGameObjects[i].GetComponent<UserStats>();  // UserStats 컴포넌트
            NavMeshAgent agent = playerGameObjects[i].GetComponent<NavMeshAgent>();  // NavMeshAgent 컴포넌트
            PlayerMove playerMove = playerGameObjects[i].GetComponent<PlayerMove>();  // PlayerMove 컴포넌트

            tempPlayerStats.Add(stats);
            tempAgents.Add(agent);
            tempPlayerMoves.Add(playerMove);
        }

        
        players = Playerss.ToList();
        playerStats = tempPlayerStats;
        agents = tempAgents;
        playerMoves = tempPlayerMoves;
    }

    [PunRPC]
    void OnBattleStart()
    {
        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ProfileSet", RpcTarget.All);
        }
        battleUI.SetActive(true);
    }

    [PunRPC] // 프로필 UI 생성
    void ProfileSet()
    {
        if (players.Count > 0)
        {
            Vector3 startPosition = profileUI.transform.position;

            for (int i = 0; i < players.Count; i++)
            {
                GameObject profile = Instantiate(profileUI, startPosition, Quaternion.identity);
                profile.transform.SetParent(battleUI.transform, false);
                startPosition.x += 400; // 간격

                profiles.Add(profile);

                ProfileSet profileSet = profile.GetComponent<ProfileSet>();
                profileSet.NicknameSet(playerBatList.battlePlayers[i].nickname);
                profileSet.HpBarInit(playerBatList.battlePlayers[i].health);
                profileSet.SetSelectImage(0); // 선택안함
                TurnCheckSystem.Instance.profiles = profiles;
            }
        }
    }

    [PunRPC]
    void MoveToBattlePos(int playerIndex)
    {
        if (playerIndex < players.Count && playerIndex < battlePos.Count)
        {
            if (playerIndex >= 0 && playerIndex < players.Count && playerIndex < battlePos.Count)
            {
                agents[playerIndex].enabled = false;
                playerMoves[playerIndex].clickMovementEnabled = false;
                players[playerIndex].transform.position = battlePos[playerIndex].position;
                players[playerIndex].transform.rotation = battlePos[playerIndex].rotation;
            }

        }
    }
}
