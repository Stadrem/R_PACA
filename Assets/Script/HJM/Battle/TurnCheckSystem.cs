using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem Instance { get; private set; }

    [Header("턴 순환")]
    public int currentTurnIndex = 0;
    public int totalPlayers;
    public bool isMyTurn = false;

    [Header("턴 UI")]
    public Button attackBtn;
    public Button defenseBtn;

    public int diceDamage;

    public List<GameObject> profiles;

    TurnFSM turnFSM;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("현재 플레이어 수: " + totalPlayers);

        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);

        turnFSM = GetComponent<TurnFSM>();

        // 마스터 클라이언트만 게임 시작
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새로운 플레이어가 들어왔을 때 totalPlayers를 갱신
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("새로운 플레이어 입장: " + newPlayer.NickName);
        Debug.Log("현재 플레이어 수: " + totalPlayers);
    }

    void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);
    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
        if (turnFSM.turnState == ActionTurn.Player)
        {
            currentTurnIndex = turnIndex;

            Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
            isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

            if (isMyTurn)
            {
                Debug.Log("내 턴");
                EnableBatUI();
                SetActiveTrueBatUI();
            }
            else
            {
                Debug.Log("다른 플레이어의 턴");
                DisableBatUI();
                SetActiveFalseBatUI();
            }
        }
        else if (turnFSM.turnState == ActionTurn.Enemy)
        {
            // 몬스터 턴
            if (PhotonNetwork.IsMasterClient)
            {
                MonsterTurnRPCCall();
            }
        }
    }

    public void EndTurn()
    {
        if (!isMyTurn) return;

            //FinishMyTask();
            photonView.RPC("FinishMyTask", RpcTarget.All);

        // 플레이어 턴에서만 순서를 진행
        if (turnFSM.turnState == ActionTurn.Player)
        {
            photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 0);
            currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;

            // 모든 플레이어의 턴이 끝나면 몬스터 턴으로 전환
            if (currentTurnIndex == 0)
            {
                photonView.RPC("ChangeTurnToEnemy", RpcTarget.All);
            }
            else
            {
                //BeginTurn(currentTurnIndex);
                photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
            }
        }
    }

    [PunRPC]
    void FinishMyTask()
    {
        turnFSM.finishActionCount++;
    }

    [PunRPC]
    public void ChangeTurnToEnemy()
    {
        turnFSM.turnState = ActionTurn.Enemy;
        BeginTurn(-1); // 몬스터 턴 시작
    }

    public void OnClickAttack()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby(); // 주사위 보이게
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값 3 넣음
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);
        StartCoroutine(AttackCallRPC());
    }

    public void OnClickDefense()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby(); // 주사위 보이게
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값 3 넣음
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
        StartCoroutine(DefenseCallRPC());
    }

    private IEnumerator AttackCallRPC()
    {
        yield return new WaitForSeconds(2f);

        if (diceDamage >= 4)
        {
            photonView.RPC("DiceAttackSuccess", RpcTarget.All, diceDamage);
        }
        else
        {
            photonView.RPC("DiceAttackFail", RpcTarget.All, diceDamage);
        }
        yield return new WaitForSeconds(3f);
        EndTurn();
    }

    private IEnumerator DefenseCallRPC()
    {
        yield return new WaitForSeconds(2f);

        if (diceDamage >= 4)
        {
            photonView.RPC("DiceDefenseSuccess", RpcTarget.All, diceDamage);
        }
        else
        {
            photonView.RPC("DiceDefenseFail", RpcTarget.All, diceDamage);
        }
        yield return new WaitForSeconds(3f);
        EndTurn();
    }

    public void MonsterTurnRPCCall()
    {
        photonView.RPC("MonsterTurnStart", RpcTarget.All);
    }

    [PunRPC]
    public void MonsterTurnStart()
    {
        Debug.Log("몬스터 턴 시작");
        StartCoroutine(MonsterAction());
    }

    private IEnumerator MonsterAction()
    {
        Debug.Log("몬스터 행동 시작");
        BattleManagerCopy.Instance.enemyAnim.SetTrigger("Rage");

        yield return new WaitForSeconds(2f);
        photonView.RPC("ChangeTurnToPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeTurnToPlayer()
    {
        turnFSM.turnState = ActionTurn.Player;
        //StartGame(); dmdkdkdk아아아으ㅏ앙어앙아앙ㅇ앙 제발 두번씩 호출되지말아줘ㅣ...
        // 턴 FSM을 싱글톤으로 빼서 카운트 누적하고 값 받아와서 할까...
        BeginTurn(0);
        //photonView.RPC("BeginTurn", RpcTarget.All, 0); // 첫 번째 플레이어의 턴으로 복귀

    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[playerIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
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
}
