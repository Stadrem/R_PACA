using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

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



    [Header("적 NPC")]
    public GameObject enemy;
    public Animator enemyAnim;
    public Slider enemyHPBar;

    private int turnCount = 1;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle();
        }

        if (players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            InitializePlayers();
        }
    }

    public void StartBattle()
    {
        playerBatList = GetComponent<PlayerBatList>();
        // ProfileSet();
        photonView.RPC("OnBattleStart", RpcTarget.All);
    }

    private void InitializePlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        //print("플레이어 오브젝트 수 : " + playerGameObjects.Length.ToString());
        GameObject[] Playerss = new GameObject[playerGameObjects.Length];

        List<UserStats> tempPlayerStats = new List<UserStats>();
        List<NavMeshAgent> tempAgents = new List<NavMeshAgent>();
        List<PlayerMove> tempPlayerMoves = new List<PlayerMove>();
        List<Animator> tempPlayerAnims = new List<Animator>();

        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            PhotonView pv = playerGameObjects[i].GetComponent<PhotonView>();
            int playerindex = pv.ViewID / 1000;
            //print(playerindex);

            // 플레이어 게임 오브젝트 리스트에 추가
            Playerss[playerindex - 1] = playerGameObjects[i];

            // 컴포넌트들 가져오기
            UserStats stats = playerGameObjects[i].GetComponent<UserStats>();
            NavMeshAgent agent = playerGameObjects[i].GetComponent<NavMeshAgent>();
            PlayerMove playerMove = playerGameObjects[i].GetComponent<PlayerMove>();
            Animator playerAnim = playerGameObjects[i].GetComponentInChildren<Animator>();

            tempPlayerStats.Add(stats);
            tempAgents.Add(agent);
            tempPlayerMoves.Add(playerMove);
            tempPlayerAnims.Add(playerAnim);
        }


        players = Playerss.ToList();
        playerStats = tempPlayerStats;
        agents = tempAgents;
        playerMoves = tempPlayerMoves;
        playerAnims = tempPlayerAnims;
    }

    [PunRPC]
    void OnBattleStart()
    {
        PlayUniverseManager.Instance.RPC_FinishConversation();
        PlayUniverseManager.Instance.isBattle = true;
        var gameObject = PlayUniverseManager.Instance.NpcManager.currentInteractNpc?.gameObject;
        if (gameObject == null)
        {
            gameObject = GameObject.Find("NPC_Golem(Clone)");
        }
        SetEnemy(gameObject);
        playerBatList = GetComponent<PlayerBatList>();

        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
            ProfileSet();
        }

        playerBatList = GetComponent<PlayerBatList>();
        battleUI.SetActive(true);

    }

    [PunRPC] // 프로필 UI 생성
    void ProfileSet()
    {
        Debug.Log($"start");
        if (players.Count > 0)
        {
            Debug.Log($"플레이어 수 : {players.Count}");
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

    // ------------------------- 전투 시작 초기 세팅 --------------------------


    // ------------------------- 전투 결과 세팅 -------------------------------

    // 주사위 공격 성공
    [PunRPC]
    public void DiceAttackSuccess(int damage)
    {
        enemyAnim.SetTrigger("Hit");
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
        ShowBattleUI("공격 성공!"); // 공격 성공 UI
        NextTurn();
    }

    // 주사위 공격 실패
    [PunRPC]
    public void DiceAttackFail()
    {
        enemyAnim.SetTrigger("Defense");
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        ShowBattleUI("공격 실패"); // 공격 실패 UI
        NextTurn();
    }

    // 주사위 방어 성공
    [PunRPC]
    public void DiceDefenseSuccess(int damage)
    {
        enemyAnim.SetTrigger("Attack"); // 몬스터 Attack 트리거
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Defense"); // 플레이어 Defense 트리거
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage / 2); // 데미지 절반
        ShowBattleUI("방어 성공!"); // 방어 성공 UI
        NextTurn();
    }

    // 주사위 방어 실패
    [PunRPC]
    public void DiceDefenseFail(int damage)
    {
        enemyAnim.SetTrigger("Attack"); // 몬스터 Attack 트리거
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Hit"); // 플레이어 Hit 트리거
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage); // 플레이어 체력 감소
        ShowBattleUI("방어 실패"); // 방어 실패 UI
        NextTurn();
    }

    [PunRPC]
    public void UpdateEnemyHealth(int damage)
    {
        enemyHPBar.value = enemyHPBar.value - damage; // 적 체력 감소
    }

    private void ShowBattleUI(string message)
    {
        currentTurnTXT.text = message;
        nextTurnUI.SetActive(true); // 다음턴 UI 표시
    }

    private void NextTurn()
    {
        turnCount++;
        currentTurnTXT.text = "전투 " + turnCount + "턴";
        StartCoroutine(HideNextTurnUI());
    }

    private IEnumerator HideNextTurnUI()
    {
        yield return new WaitForSeconds(2f);
        nextTurnUI.SetActive(false);
    }

    public void SetEnemy(GameObject enemy)
    {
        var anim = enemy.GetComponentInChildren<Animator>();
        this.enemy = enemy;
        enemyAnim = anim;
    }

}
