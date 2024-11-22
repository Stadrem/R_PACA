using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class BattleManagerCopy : MonoBehaviourPunCallbacks
{
    public static BattleManagerCopy Instance { get; private set; }

    [Header("플레이어 리스트")]
    public List<GameObject> players = new List<GameObject>();  // 플레이어 목록
    public List<UserStats> playerStats = new List<UserStats>();  // 플레이어 스탯 정보 목록
    public List<Transform> battlePos;                          // 전투 시 이동 위치
    public List<NavMeshAgent> agents = new List<NavMeshAgent>();
    public List<PlayerMove> playerMoves = new List<PlayerMove>();
    public List<Animator> playerAnims = new List<Animator>();

    public List<GameObject> profiles;
    public PlayerBatList playerBatList;

    [Header("UI 표시")]
    public GameObject battleUI;
    public GameObject profileUI;
    public GameObject nextTurnUI;
    public TMP_Text currentTurnTXT;
    public RectTransform profileParent;


    [Header("적 NPC")]
    public GameObject enemy;
    public Animator enemyAnim;
    public Slider enemyHPBar;

    private int turnCount = 1;

    public bool isBattle = false;

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
        enemyHPBar.maxValue = 25;
        enemyHPBar.value = enemyHPBar.maxValue;

        profileParent = GameObject.Find("Panel_Profiles").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle();
            photonView.RPC("IsBattle", RpcTarget.All);
        }

        if (players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            InitializePlayers();
        }
    }
    [PunRPC]
    public void IsBattle()
    {
        isBattle = true;
        if (PhotonNetwork.IsMasterClient)
        {
            TurnCheckSystem.Instance.StartGame();
        }
    }

    public void StartBattle()
    {
        playerBatList = GetComponent<PlayerBatList>();
        photonView.RPC("OnBattleStart", RpcTarget.All);
    }

    private void InitializePlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] Playerss = new GameObject[playerGameObjects.Length];

        // 임시 배열들
        UserStats[] tempPlayerStats = new UserStats[playerGameObjects.Length];
        NavMeshAgent[] tempAgents = new NavMeshAgent[playerGameObjects.Length];
        PlayerMove[] tempPlayerMoves = new PlayerMove[playerGameObjects.Length];
        Animator[] tempPlayerAnims = new Animator[playerGameObjects.Length];

        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            PhotonView pv = playerGameObjects[i].GetComponent<PhotonView>();
            int playerindex = pv.ViewID / 1000 - 1; // 포톤뷰(들어온 순서대로) 정렬


            Playerss[playerindex] = playerGameObjects[i];
            tempPlayerStats[playerindex] = playerGameObjects[i].GetComponent<UserStats>();
            tempAgents[playerindex] = playerGameObjects[i].GetComponent<NavMeshAgent>();
            tempPlayerMoves[playerindex] = playerGameObjects[i].GetComponent<PlayerMove>();
            tempPlayerAnims[playerindex] = playerGameObjects[i].GetComponentInChildren<Animator>();
        }

        // 리스트로 다시 넣어주기
        players = Playerss.ToList();
        playerStats = tempPlayerStats.ToList();
        agents = tempAgents.ToList();
        playerMoves = tempPlayerMoves.ToList();
        playerAnims = tempPlayerAnims.ToList();
    }

    [PunRPC]
    void OnBattleStart()
    {
        //PlayUniverseManager.Instance.RPC_FinishConversation();
        //PlayUniverseManager.Instance.isBattle = true;
        //var gameObject = PlayUniverseManager.Instance.NpcManager.currentInteractNpc?.gameObject;
        //if (gameObject == null)
        //{
        //GameObject gameObject = GameObject.Find("NPC_Golem(Clone)");
        //}

        if (enemy == null)
        {
            GameObject gameObject = GameObject.Find("NPC_Golem(Clone)");
            enemy = gameObject;
        }

        SetEnemy(enemy);
        playerBatList = GetComponent<PlayerBatList>();

        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
        }

        ProfileSet();
        battleUI.SetActive(true);

    }

    [PunRPC] // 프로필 UI 생성
    void ProfileSet()
    {
        Debug.Log($"start");
        if (players.Count > 0)
        {
            Debug.Log($"플레이어 수 : {players.Count}");
            // Vector3 startPosition = profileUI.transform.position;

            for (int i = 0; i < players.Count; i++)
            {
                GameObject profile = Instantiate(profileUI, Vector3.zero, Quaternion.identity, profileParent);
                // startPosition.x += 400; // 간격 -> LayoutGroup 사용하면 필요없음

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


    // --------------------------------------------------------
    // 전투 결과 세팅

    // 주사위 공격 성공
    [PunRPC]
    public IEnumerator DiceAttackSuccess(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        yield return new WaitForSeconds(0.37f);
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
    }

    // 주사위 공격 실패
    [PunRPC]
    public IEnumerator DiceAttackFail(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        yield return new WaitForSeconds(0.37f);
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트

    }

    // 주사위 방어 성공
    [PunRPC]
    public void DiceDefenseSuccess(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage / 2); // 데미지 절반
    }

    // 주사위 방어 실패
    [PunRPC]
    public void DiceDefenseFail(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage); // 플레이어 체력 감소
    }

    [PunRPC]
    public void UpdateEnemyHealth(int damage)
    {
        enemyHPBar.value = enemyHPBar.value - damage; // 적 체력 감소
    }
    [PunRPC]
    public void TurnTXTUpdate()
    {
        turnCount++;
        currentTurnTXT.text = "전투 " + turnCount + "턴";
    }

    public void SetEnemy(GameObject enemy)
    {
        var anim = enemy.GetComponentInChildren<Animator>();
        this.enemy = enemy;
        enemyAnim = anim;
    }

}
