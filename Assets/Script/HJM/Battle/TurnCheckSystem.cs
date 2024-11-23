using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Profiling;
using Cinemachine;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem Instance { get; private set; }

    [Header("턴 순환")]
    public int currentTurnIndex = 0;
    public int totalPlayers;
    public bool isMyTurn = false;

    [Header("턴 카메라")]
    public CinemachineVirtualCamera vCam;
    public Transform vCamTarget;

    [Header("턴 UI")]
    public Button attackBtn;
    public Button defenseBtn;

    public int diceDamage;

    public List<GameObject> profiles;
    public List<GameObject> turnLight;



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

        GameObject turnFSMObject = GameObject.Find("---TurnFSM---");
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
        if (BattleManager.Instance.isBattle)
        {
            currentTurnIndex = turnIndex;
            //photonView.RPC("ProfileLight", RpcTarget.AllBuffered, currentTurnIndex, true);
            if (turnFSM.turnState == ActionTurn.Player)
            {
                Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
                isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;
                TXTTurnCount();
                if (isMyTurn)
                {
                    Debug.Log("내 턴");
                    EnableBatUI();
                    SetActiveTrueBatUI();
                    photonView.RPC("WaitStartLight", RpcTarget.AllBuffered, currentTurnIndex, true);
                    photonView.RPC("LookAtTarget", RpcTarget.All, currentTurnIndex);
                }
                else
                {
                    Debug.Log("다른 플레이어의 턴");
                    DisableBatUI();
                    SetActiveFalseBatUI();
                    photonView.RPC("LookAtTarget", RpcTarget.All, currentTurnIndex);
                }
            }
            else if (turnFSM.turnState == ActionTurn.Enemy)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("LookAtTarget", RpcTarget.All, -1);
                    MonsterTurnRPCCall();
                }
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
            photonView.RPC("ProfileLight", RpcTarget.All, playerIndex, isOn);
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
            turnLight[currentTurnIndex].SetActive(false);
            

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

    public void TXTTurnCount()
    {
        TurnFSM.Instance.turnCount.text = $"전투 {TurnFSM.Instance.finishActionCount + 1}턴";
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
        photonView.RPC("LookAtTarget", RpcTarget.All, -1);
        StartCoroutine(MonsterAction());
    }

    private IEnumerator MonsterAction()
    {
        Debug.Log("몬스터 행동 시작");
        BattleManager.Instance.MonsterAttack(4); // 임의값
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
        if (BattleManager.Instance.isBattle == false) return;
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
        if (BattleManager.Instance.isBattle)
        {
            profiles[playerIndex].GetComponent<ProfileSet>().LightProfile(isOn);
            turnLight[playerIndex].SetActive(isOn);
        }
    }

    [PunRPC]
    public void LookAtTarget(int playerIndex)
    {
        vCamTarget = vCam.LookAt;
        if (playerIndex >= 0 && playerIndex < BattleManager.Instance.players.Count)
        {
            vCam.LookAt = BattleManager.Instance.players[playerIndex].transform;
        }
        else if (playerIndex == -1)
        {
            vCam.LookAt = BattleManager.Instance.enemy.transform;
        }
    }

}

