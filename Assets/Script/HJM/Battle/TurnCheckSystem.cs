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
        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);


        totalPlayers = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
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
        DisableBatUI();
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(5f);

        // diceDamage가 0이면 공격 실패, 그렇지 않으면 성공
        if (diceDamage > 0)
        {
            // 공격 성공
            print("공격 성공");
        }
        else
        {
            print("공격 실패");
        }
        diceDamage = 0;
        
        Debug.Log("턴 선택 행동 끝, 몬스터 턴");
        EndTurn(); // 턴 종료
    }

    void StartGame()
    {
        BeginTurn(currentTurnIndex);

    }

    void BeginTurn(int turnIndex)
    {
        currentTurnIndex = turnIndex;
        print("턴 시작 currentTurnIndex값은" + currentTurnIndex);
        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
        isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

        if (isMyTurn)
        {
            Debug.Log("내 턴");
            SetActiveTrueBatUI();
            EnableBatUI();
        }
        else
        {
            Debug.Log("내 턴 아님");
            SetActiveFalseBatUI();
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
        yield return new WaitForSeconds(3f);
        // 몬스터 행동 처리
        StartCoroutine(MonsterAction());

    }

    private IEnumerator MonsterAction()
    {
        Debug.Log("몬스터 행동");
        yield return new WaitForSeconds(2f);

        Debug.Log("몬스터 행동끝");

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
        PerformAction();
    }

    public void OnClickDefense()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
        PerformAction();
    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[currentTurnIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
    }


    public void SetActiveTrueBatUI()
    {
        attackBtn.gameObject.SetActive(true);
        defenseBtn.gameObject.SetActive(true);
    }

    public void SetActiveFalseBatUI()
    {

        attackBtn.gameObject.SetActive(false);
        defenseBtn.gameObject.SetActive(false);
    }


    public void EnableBatUI()
    {
        attackBtn.interactable = true;
        defenseBtn.interactable = true;
    }

    public void DisableBatUI()
    {
        attackBtn.interactable = false;
        defenseBtn.interactable = false;
    }
}