using System;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using Cinemachine;
using Data.Models.Universe.Characters;
using Unity.VisualScripting;
using UniversePlay;
using ViewModels;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager Instance { get; private set; }

    [Header("플레이어 리스트")]
    public List<GameObject> players = new List<GameObject>(); // 플레이어 목록

    public List<UserStats> playerStats = new List<UserStats>(); // 플레이어 스탯 정보 목록
    public List<Transform> battlePos; // 전투 시 이동 위치
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
    public GameObject popBatStart;
    public GameObject popBatEnd;

    [Header("전투위치")]
    public GameObject turnEffectPrefab;

    public float offset = 2.0f;

    [Header("적 NPC")]
    public GameObject enemy;

    public int enemyHP = 10;
    public int enemyDamage = 3;
    public Animator enemyAnim;
    public Slider enemyHPBar;
    public TMP_Text enemyHpTXT;
    public int targetPlayerIndex; // 공격 대상 플레이어


    [Header("카메라")]
    public CinemachineVirtualCamera vCam;
    public CinemachineVirtualCamera vCineCam;
    public Vector3 cineOffset;

    public bool isBattle = false;
    public bool isWin = false;
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
        EnemyHPStart(enemyHP); // 적 체력 설정
        profileParent = GameObject.Find("Panel_Profiles").GetComponent<RectTransform>();

        turnEffectPrefab = Resources.Load<GameObject>("TurnEffect"); // 이펙트 리소스(리소스폴더안에)
        print("TurnEffect 리소스 가져왔어요");

        if (enemy != null)
        {
        }
        else
        {
            print("몬스터 못찾음");
        }
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
            GameObject gameObject = GameObject.Find("NPC_Golem(Clone)"); // 나중에는 다른 방법으로 할당
            enemy = gameObject;
            SetEnemy(enemy);
            GenerateBattlePositions();
        }

        if (vCineCam != null)
        {
            CineCamaraPosSet();
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
        PlayUniverseManager.Instance.NpcManager.isBlocked = true;
        //SetEnemy(enemy);
        playerBatList = GetComponent<PlayerBatList>();
        for (int i = 0; i < players.Count; i++)
        {
            photonView.RPC("MoveToBattlePos", RpcTarget.All, i);
        }

        ProfileSet();
        
        BattleCinemachine.Instance.StartAwakeCinema();
        //CineCam(true);
        //battleUI.SetActive(true);
    }

    public void CallRPCIsBattle()
    {
        photonView.RPC("IsBattle", RpcTarget.All);
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
    // 마스터클라이언트만 전투위치 랜덤으로 뽑아서 전송
    void GenerateBattlePositions()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        print("적 위치 잡음");
        Vector3 enemyPos = enemy.transform.position;
        Vector3 enemyForward = enemy.transform.forward;
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        battlePos.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            Vector3 pos;
            Quaternion rotation;
            bool isOverlapping;

            do
            {
                isOverlapping = false;

                float randomX = Random.Range(2.0f, 3.0f);
                float randomZ = Random.Range(3.0f, 4.0f);
                pos = enemyPos + new Vector3(randomX, 0, randomZ);

                // 기존 배틀 포지션들과 충돌하는지 확인
                foreach (Transform existingTransform in battlePos)
                {
                    if (Vector3.Distance(pos, existingTransform.position) < offset)
                    {
                        isOverlapping = true;
                        break;
                    }
                }
            } while (isOverlapping);

            // 플레이어가 몬스터를 바라보도록
            Vector3 directionToEnemy = (enemyPos - pos).normalized;
            rotation = Quaternion.LookRotation(directionToEnemy);

            // 다른 유저들에게 뽑은 인덱스 위치와 회전값 전송
            photonView.RPC("AddBattlePosRPC", RpcTarget.AllBuffered, i, pos, rotation);

            //print($"BattlePos_{i} 위치 및 회전 설정 및 동기화 완료");
        }
    }


    [PunRPC]
    // 받은 인덱스, 위치, 회전값으로 전투위치와 이펙트를 생성하고 리스트에 추가
    void AddBattlePosRPC(int index, Vector3 position, Quaternion rotation)
    {
        // BattlePos 생성
        Transform posTransform = Instantiate(Resources.Load<Transform>("BattlePos")) as Transform;
        posTransform.name = $"BattlePos_{index}";
        posTransform.position = position;
        posTransform.rotation = rotation;

        // 배틀 포지션 리스트에 추가
        battlePos.Add(posTransform);

        // 이펙트 생성하고 리스트에 추가
        GameObject turnEffect = Instantiate(turnEffectPrefab, posTransform.position, Quaternion.identity);
        turnEffect.name = $"turnEffect_{index}";
        turnEffect.SetActive(false);
        TurnCheckSystem.Instance.turnLight.Add(turnEffect);

        //print($"BattlePos_{index} 생성 및 리스트 추가 완료");
    }


    [PunRPC] // 받은 전투위치로 플레이어 이동하고, 클릭이동 잠금
    void MoveToBattlePos(int playerIndex)
    {
        if (playerIndex < players.Count && playerIndex < battlePos.Count)
        {
            agents[playerIndex].enabled = false;
            playerMoves[playerIndex].clickMovementEnabled = false;

            players[playerIndex].transform.position = battlePos[playerIndex].position;
            players[playerIndex].transform.rotation = battlePos[playerIndex].rotation;

            //print($"Player_{playerIndex} 이동 완료");
        }
    }


    public void CineCamaraPosSet()
    {
        vCineCam.transform.position = enemy.transform.position + cineOffset;
    }




    // -------------------------------------------------------- 전투 결과 세팅


    [PunRPC] // 주사위 공격 성공
    public IEnumerator DiceAttackSuccess(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack");
        yield return new WaitForSeconds(0.37f);
        SoundManager.Get().PlaySFX(5); // 플레이어 공격효과음
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
    }


    [PunRPC] // 주사위 공격 실패
    public IEnumerator DiceAttackFail(int damage)
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Attack"); // 공격실패하는 바보 애니메이션 넣기
        yield return new WaitForSeconds(0.37f);
        SoundManager.Get().PlaySFX(5); // 플레이어 공격효과음
        enemyAnim.SetTrigger("Damage");
        UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
    }


    [PunRPC] // 주사위 방어 성공
    public void DiceDefenseSuccess(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>()
            .DamagedPlayer(damage / 2); // 데미지 절반
        // 근데 방어가 좀 이상하긴 함... 공격하면 공격 안당하는데 방어하면 공격당함 방패같은거라도 세워놔야하나
    }


    [PunRPC] // 주사위 방어 실패
    public void DiceDefenseFail(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex].SetTrigger("Damage");
        profiles[TurnCheckSystem.Instance.currentTurnIndex].GetComponent<ProfileSet>()
            .DamagedPlayer(damage); // 플레이어 체력 감소
    }

    [PunRPC]
    public void SetTargetPlayer()
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 타겟대상플레이어 추첨
        {
            targetPlayerIndex = Random.Range(0, playerAnims.Count);
            photonView.RPC("SyncTargetPlayer", RpcTarget.All, targetPlayerIndex);
        }
    }

    [PunRPC]
    public void SyncTargetPlayer(int index)
    {
        targetPlayerIndex = index;
    }

    [PunRPC]
    public void MonsterAttack(int damage)
    {
        enemyAnim.SetTrigger("Hit2");
        SoundManager.Get().PlaySFX(6); // 적 공격 효과음

        playerAnims[targetPlayerIndex].SetTrigger("Damage");
        profiles[targetPlayerIndex].GetComponent<ProfileSet>().DamagedPlayer(damage);
    }

    [PunRPC]
    public void UpdateEnemyHealth(int damage)
    {
        enemyHPBar.value -= damage;
        enemyHpTXT.text = $"{enemyHPBar.value} / {enemyHPBar.maxValue}";
        if (enemyHPBar.value <= 0)
        {
            MonsterDie();
        }
    }



    [PunRPC]
    public void EndBattle() // 전투 종료
    {
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

        vCam.gameObject.SetActive(false);
        // PlayUniverseManager.Instance.NpcManager.isBlocked = false;

        var dict = new Dictionary<int, int>();
        foreach (var player in ViewModelManager.Instance.UniversePlayViewModel.UniversePlayers)
        {
            dict.Add(player.UserCode, 5); // 임시로 5로 설정
        }
        StartCoroutine(
            ViewModelManager.Instance.UniversePlayViewModel.FinishBattle(
                PlayUniverseManager.Instance.roomNumber,
                enemyHPBar.value <= 0,
                dict,
                0,
                (res) => { Debug.Log($"전투 결과 전송 완료 : {res.IsSuccess}"); }
            )
        );
        Debug.Log("전투 종료");
        print("엔딩 크레딧 올라가기 전");
        StartCoroutine(WaitEnding(4.0f, isWin));
    }
    public void PlayerDie()
    {
        playerAnims[TurnCheckSystem.Instance.currentTurnIndex + 1].SetTrigger("Damage"); // 죽는 애니로 변경
        vCam.LookAt = players[TurnCheckSystem.Instance.currentTurnIndex + 1].transform;
        vCam.gameObject.SetActive(true);
        print("플레이어 사망!. 몬스터 승리!");
        isWin = false;
        photonView.RPC("EndBattle", RpcTarget.All);
    }

    public void MonsterDie()
    {
        enemyAnim.SetTrigger("Die"); // 좀 더 요란하고 길게 죽는 애니로 변경
        vCineCam.gameObject.SetActive(true);
        print("몬스터 사망!. 플레이어 측 승리!");
        isWin = true;
        popBatEnd.SetActive(true);
        photonView.RPC("EndBattle", RpcTarget.All);
    }

    private IEnumerator WaitEnding(float waitTime, bool win)
    {
        print("엔딩 크레딧 대기중");
        yield return new WaitForSeconds(waitTime);
        Ending.Get().EnableCanvas(win);
        print("엔딩 크레딧 올라감");

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
        enemyHPBar.maxValue = hp;
        enemyHPBar.value = enemyHPBar.maxValue;
        enemyHpTXT.text = $"{enemyHPBar.value} / {enemyHPBar.maxValue}";
    }

    // 카메라 뷰 전환
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
            print("시네마틱 카메라 끄고, 대화(턴) 카메라 켰어요");
            vCineCam.gameObject.SetActive(false);
            vCam.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;
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