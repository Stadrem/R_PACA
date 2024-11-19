using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.Collections;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem Instance { get; private set; }
    public static CircularSlider circularSlider;

    [Header("턴 순환")]
    public int currentTurnIndex = 0;
    public int totalPlayers;
    public bool isMyTurn = false;

    [Header("턴 UI")]
    public Button attackBtn;
    public Button defenseBtn;
    public Button turnCompBtn;
    public GameObject turnSilder;

    public int diceDamage;

    public bool isMyTurnAction = false;

    public List<GameObject> profiles;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        circularSlider = turnSilder.GetComponent<CircularSlider>();
        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);


        totalPlayers = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    void Update()
    {
        if (isMyTurn && isMyTurnAction == true)
        {
            PerformAction();
            DisableButtons();
        }
    }
    private void PerformAction()
    {
        Debug.Log("턴 선택 행동 ~~");

        // 주사위 굴리기
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값
        print(diceDamage);

        // 굴리는거 기다림
        StartCoroutine(WaitAndEndTurn());


        Debug.Log("턴 선택 행동 끝");
        isMyTurnAction = false;
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(5f);

        // diceDamage가 0이면 공격 실패, 그렇지 않으면 성공
        if (diceDamage > 0)
        {
            // 공격 성공
            photonView.RPC("DiceAttackSuccess", RpcTarget.All, diceDamage);
            photonView.RPC("UpdateEnemyHealth", RpcTarget.All, diceDamage);
        }
        else
        {
            // 공격 실패
            photonView.RPC("DiceAttackFail", RpcTarget.All);
        }

        // diceDamage 초기화
        diceDamage = 0;

        // 턴 종료 (다른 플레이어에게 턴 넘어감)
        EndTurn();
    }

    void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);

    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
        currentTurnIndex = turnIndex;
        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];

        isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

        if (isMyTurn)
        {
            Debug.Log("내 턴");
            // 타이머 시작하기
            circularSlider.StartDepletion();
            // 선택지 버튼 활성화
            EnableButtons();
        }
        else
        {
            Debug.Log("다음 사람 턴");
            // 타이머 초기화하고 멈춰놓기
            circularSlider.ResetSlider();
            // 선택지 버튼 비 활성화
            DisableButtons();
        }
    }
    public void EndTurn()
    {
        if (!isMyTurn) return;

        isMyTurn = false;

        // 몬스터 턴 시작
        photonView.RPC("BeginMonsterTurn", RpcTarget.All);
    }

    [PunRPC]
    public void BeginMonsterTurn()
    {
        Debug.Log("몬스터 턴 시작");

        // 타이머 초기화
        circularSlider.ResetSlider();
        // 버튼 비활성화
        DisableButtons();

        // 몬스터 행동 처리
        StartCoroutine(MonsterAction());
    }

    private IEnumerator MonsterAction()
    {
        Debug.Log("몬스터 행동");
        BattleManager.Instance.enemyAnim.SetTrigger("Hit");
        // 몬스터 행동 (예: 공격)

        // 몬스터 행동 시간 지연
        yield return new WaitForSeconds(3f);

        // 행동 후 턴 종료
        photonView.RPC("EndMonsterTurn", RpcTarget.All);
    }

    [PunRPC]
    public void EndMonsterTurn()
    {
        Debug.Log("몬스터 턴 종료. 다음 플레이어의 턴으로 넘어갑니다.");

        // 다음 플레이어 턴으로 전환
        currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;
        photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
    }



    public void OnClickAttack()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);
    }

    public void OnClickDefense()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[currentTurnIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
    }


    public void EnableButtons()
    {
        attackBtn.interactable = true;
        defenseBtn.interactable = true;
        turnCompBtn.interactable = true;
    }

    public void DisableButtons()
    {
        attackBtn.interactable = false;
        defenseBtn.interactable = false;
        turnCompBtn.interactable = false;
    }
}