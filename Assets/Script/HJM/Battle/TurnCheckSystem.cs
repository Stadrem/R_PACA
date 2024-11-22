using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Profiling;

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

        GameObject turnFSMObject = GameObject.Find("---TurnFSM---"); // 나중엔 그냥 인스펙터에서 할당하자~
        turnFSM = turnFSMObject.GetComponent<TurnFSM>();


    }

    // 새로운 플레이어가 들어왔을 때 totalPlayers 갱신 함수 덮어쓰기
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("새로운 플레이어 입장: " + newPlayer.NickName);
        Debug.Log("현재 플레이어 수: " + totalPlayers);
    }

    public void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);
    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
        currentTurnIndex = turnIndex;
        //photonView.RPC("ProfileLight", RpcTarget.AllBuffered, currentTurnIndex, true);
        if (turnFSM.turnState == ActionTurn.Player)
        {
            Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
            isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

            if (isMyTurn)
            {
                Debug.Log("내 턴");
                EnableBatUI();
                SetActiveTrueBatUI();
                photonView.RPC("WaitStartLight", RpcTarget.AllBuffered, currentTurnIndex, true);
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
            if (PhotonNetwork.IsMasterClient)
            {
                MonsterTurnRPCCall();
            }
        }
    }

    [PunRPC]
    public IEnumerator WaitStartLight(int playerIndex, bool isOn)
    {
        yield return new WaitForSeconds(0.1f);
        if (isMyTurn)
        {
            print("내 프로필 불 킴");
            photonView.RPC("ProfileLight", RpcTarget.AllBuffered, playerIndex, isOn);
        }
    }


    public void EndTurn()
    {
        if (!isMyTurn) return;

        photonView.RPC("FinishMyTask", RpcTarget.All);

        if (turnFSM.turnState == ActionTurn.Player)
        {
            photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 0);
            photonView.RPC("ProfileLight", RpcTarget.AllBuffered, currentTurnIndex, false);
            currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;

            if (currentTurnIndex == 0)
            {
                photonView.RPC("ChangeTurnToEnemy", RpcTarget.All);
            }
            else
            {
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
        BeginTurn(-1);
    }

    public void OnClickAttack()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby();
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3);
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);
        StartCoroutine(AttackCallRPC());
    }

    public void OnClickDefense()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby();
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3);
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
        StartCoroutine(DefenseCallRPC());
    }

    private IEnumerator AttackCallRPC()
    {
        yield return new WaitForSeconds(3f);

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
        yield return new WaitForSeconds(3f);

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
        BeginTurn(0);
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

    [PunRPC] 
    public void ProfileLight(int playerIndex, bool isOn)
    {
        if (BattleManagerCopy.Instance.isBattle)
        {
            profiles[playerIndex].GetComponent<ProfileSet>().LightProfile(isOn);
        }
    }
}

