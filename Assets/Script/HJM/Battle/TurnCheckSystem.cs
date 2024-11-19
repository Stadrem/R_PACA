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
            DisableBatUI();

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
        Debug.Log("턴 선택 행동 끝, 몬스터 턴");
        EndTurn(); // 턴 종료
    }

    void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);

    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
        currentTurnIndex = turnIndex;
        print("턴 시작 currentTurnIndex값은" + currentTurnIndex);
        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
        isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

        if (isMyTurn)
        {
            Debug.Log("내 턴");
            EnableBatUI();
        }
        else
        {
            Debug.Log("내 턴 아님");
            // 선택지 버튼 비 활성화
            DisableBatUI();
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
    public IEnumerator BeginMonsterTurn()
    {
        Debug.Log("몬스터 턴 시작");
        // 버튼 비활성화
        DisableBatUI();
        yield return new WaitForSeconds(3f);
        // 몬스터 행동 처리
        StartCoroutine(MonsterAction());

    }

    private IEnumerator MonsterAction()
    {
        Debug.Log("몬스터 행동");
        BattleManager.Instance.enemyAnim.SetTrigger("Hit2");
        Debug.Log("몬스터 행동끝");

        yield return new WaitForSeconds(2f);

        // 행동 후 턴 종료
        photonView.RPC("EndMonsterTurn", RpcTarget.All);
    }

    [PunRPC]
    public void EndMonsterTurn()
    {
            Debug.Log("몬스터 턴 종료. 다음 플레이어의 턴으로 넘어갑니다.");

            currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;

            print("currentTurnIndex: " + currentTurnIndex);

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


    public void EnableBatUI()
    {
        circularSlider.StartDepletion();
        attackBtn.interactable = true;
        defenseBtn.interactable = true;
        turnCompBtn.interactable = true;
    }

    public void DisableBatUI()
    {
        circularSlider.ResetSlider();
        attackBtn.interactable = false;
        defenseBtn.interactable = false;
        turnCompBtn.interactable = false;
    }
}