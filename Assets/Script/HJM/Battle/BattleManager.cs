using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;

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
    public RectTransform profileParent;


    [Header("적 NPC")]
    public GameObject enemy;
    public Animator enemyAnim;
    public Slider enemyHPBar;
    public TMP_Text enemyHpTXT;


    [Header("카메라")]
    public CinemachineVirtualCamera vCam;
    public CinemachineVirtualCamera vCineCam;

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
        EnemyHPStart(50); // 적 체력 설정
        profileParent = GameObject.Find("Panel_Profiles").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // 전투 바로 시작하게 만든 키
        {
            StartBattle();
            photonView.RPC("IsBattle", RpcTarget.All);
        }

        if (players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            InitializePlayers();
        }

        if (enemy == null)
        {
            GameObject gameObject = GameObject.Find("NPC_Golem(Clone)");
            enemy = gameObject;
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

        SetEnemy(enemy);
        playerBatList = GetComponent<PlayerBatList>();

        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
        }

        ProfileSet();

        //BattleCinemachine.cs에서 처리
        BattleCinemachine.Instance.StartAwakeCinema();
        //CineCam(true);
        //battleUI.SetActive(true);
        //photonView.RPC("IsBattle", RpcTarget.All);
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


    [PunRPC] // 프로필 UI 생성
    void ProfileSet()
    {
        Debug.Log($"start");
        if (players.Count > 0)
        {
            Debug.Log($"플레이어 수 : {players.Count}");

            for (int i = 0; i < players.Count; i++)
            {
                GameObject profile = Instantiate(profileUI, Vector3.zero, Quaternion.identity, profileParent);

                profiles.Add(profile);
                ProfileSet profileSet = profile.GetComponent<ProfileSet>();
                profileSet.NicknameSet(playerBatList.battlePlayers[i].nickname);
                profileSet.HpBarInit(playerBatList.battlePlayers[i].health);
                profileSet.SetSelectImage(0); // 선택안함
                TurnCheckSystem.Instance.profiles = profiles;
            }
        }
    }

    [PunRPC] // 나중에 NPC기준으로 생성하게 할까.. 일단은 미리 배치 
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


    // -------------------------------------------------------- 전투 결과 세팅


    // 주사위 공격 성공
    [PunRPC]
    public IEnumerator DiceAttackSuccess(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        yield return new WaitForSeconds(0.37f);
        SoundManager.Get().PlaySFX(5); // 플레이어 공격효과음
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
    }

    // 주사위 공격 실패
    [PunRPC]
    public IEnumerator DiceAttackFail(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack"); // 공격실패하는 바보 애니메이션 넣기
        yield return new WaitForSeconds(0.37f);
        SoundManager.Get().PlaySFX(5); // 플레이어 공격효과음
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트

    }

    // 주사위 방어 성공
    [PunRPC]
    public void DiceDefenseSuccess(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage / 2); // 데미지 절반
        // 근데 방어가 좀 이상하긴 함... 공격하면 공격 안당하는데 방어하면 공격당함 방패같은거라도 세워놔야하나
    }

    // 주사위 방어 실패
    [PunRPC]
    public void DiceDefenseFail(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>().DamagedPlayer(damage); // 플레이어 체력 감소
    }

    [PunRPC] // 몬스터가 플레이어를 공격
    public void MonsterAttack(int damage)
    {
        int randomIndex = Random.Range(0, playerAnims.Count);

        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음
        playerAnims[randomIndex].SetTrigger("Damage");
        profiles[randomIndex].GetComponent<ProfileSet>().DamagedPlayer(damage);
    }


    [PunRPC]
    public void UpdateEnemyHealth(int damage)
    {

        enemyHPBar.value -= damage;
        enemyHpTXT.text = $"{enemyHPBar.value} / {enemyHPBar.maxValue}";
        if (enemyHPBar.value <= 0)
        {
            enemyAnim.SetTrigger("Die");
            print("몬스터 사망!");
            EndBattleMonsterDie();
        }
    }
    private void EndBattleMonsterDie()
    {
        // 전투 종료
        isBattle = false;

        battleUI.SetActive(false);

        foreach (var profile in profiles)
        {
            Destroy(profile);
        }
        profiles.Clear();
        foreach (var turnLight in TurnCheckSystem.Instance.turnLight)
        {
            turnLight.SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            agents[i].enabled = true;
            playerMoves[i].clickMovementEnabled = true;
        }
        Debug.Log("전투 종료. 플레이어 측 승리!");
    }

    // 일단 만들어만 놓음
    public void EndBattlePlayerDie()
    {

        Ending.Get().EnableCanvas();

    }

    public void SetEnemy(GameObject enemy)
    {
        var anim = enemy.GetComponentInChildren<Animator>();
        this.enemy = enemy;
        enemyAnim = anim;
        vCam = enemy.GetComponentInChildren<InGameNpc>().ncVcam;
        TurnCheckSystem.Instance.vCam = vCam;
    }
    void EnemyHPStart(int hp)
    {
        enemyHPBar.maxValue = 50;
        enemyHPBar.value = enemyHPBar.maxValue;
        enemyHpTXT.text = $"{enemyHPBar.value} / {enemyHPBar.maxValue}";
    }

    public void CineCam(bool isCine)
    {
        if (isCine)
        {
            print("대화 카메라 끄고, 시네마틱 카메라 켰어요");
            vCam.gameObject.SetActive(false);
            Transform target = FindDeepChild(enemy.transform, "Bn_1");
            if (target != null)
            {
                vCineCam.LookAt = target;
                print("vCineCam.LookAt 설정 완료 " + target.name);
            }
            else
            {
                print("못찾았다!");
            }
            vCineCam.gameObject.SetActive(true);
        }
        else
        {
            print("시네마틱 카메라 끄고, 대화(턴) 카메라 켜써요");
            vCineCam.gameObject.SetActive(false);
            vCam.gameObject.SetActive(true);

        }
    }

    Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
