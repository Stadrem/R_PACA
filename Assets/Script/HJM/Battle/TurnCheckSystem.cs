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

    public bool isMonsterTurn = false;

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
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("현재 플레이어 수: " + totalPlayers);

        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);

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
        // 현재 턴 설정
        currentTurnIndex = turnIndex;
        
        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
        isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

        

        if (isMyTurn && !isMonsterTurn)
        {
            Debug.Log("내 턴");
            EnableBatUI();
            SetActiveTrueBatUI();
        }
        else
        {
            Debug.Log("다음 사람 턴");
            DisableBatUI();
            SetActiveFalseBatUI();

        }
    }

    public void EndTurn()
    {
        if (!isMyTurn) return;

        // 턴을 넘길 때는 내 턴일때만 
        if (isMyTurn)
        {
            photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 0);
            currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;
            photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
            photonView.RPC("TurnTXTUpdate", RpcTarget.All);
        }

        //// currentTurnIndex가 0일 때 몬스터 턴 시작
        //if (currentTurnIndex == 0 && !isMonsterTurn)
        //{
        //    isMonsterTurn = true;
        //    print("턴 한바퀴 돌았음, 몬스터 턴");
        //    DisableBatUI();
        //    SetActiveFalseBatUI();
        //    MonsterTurnRPCCall();  // 몬스터 턴 시작 호출
        //}
    }

    public void OnClickAttack()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby(); // 주사위 보이게
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값 3 넣음
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        // 선택이미지 업데이트
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);
        StartCoroutine(AttackCallRPC());
    }

    public void OnClickDefense()
    {
        DisableBatUI();

        DiceRollManager.Get().DiceStandby(); // 주사위 보이게
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값 3 넣음
        Debug.Log("주사위 굴린 결과: " + diceDamage);
        // 선택이미지 업데이트
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
        StartCoroutine(DefenseCallRPC());
    }

    private IEnumerator AttackCallRPC()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        if (diceDamage >= 4)
        {
            photonView.RPC("DiceAttackSuccess", RpcTarget.All, diceDamage);
        }
        else
        {
            photonView.RPC("DiceAttackFail", RpcTarget.All, diceDamage);
        }
        yield return new WaitForSeconds(3f); // 액션시간 기다리기
        EndTurn();
    }

    private IEnumerator DefenseCallRPC()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        if (diceDamage >= 4)
        {
            photonView.RPC("DiceDefenseSuccess", RpcTarget.All, diceDamage);
        }
        else
        {
            photonView.RPC("DiceDefenseFail", RpcTarget.All, diceDamage);
        }
        yield return new WaitForSeconds(3f); // 액션시간 기다리기
        EndTurn();
    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[playerIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
    }

    [PunRPC] // 프로필 인덱스에 접근하는 타이밍 다시 잡기 BeginTurn에서 호출하니까 계속 인덱스 초과뜸
    public void ProfileLight(int playerIndex, bool isActive)
    {
        if (isActive)
        {
            // 내 턴에만 불 켬
            profiles[playerIndex].GetComponent<ProfileSet>().OnLightProfile();
        }
        else
        {
            // 내 턴아니면 불 끔
            profiles[playerIndex].GetComponent<ProfileSet>().OffLightProfile();
        }

    }

    public void MonsterTurnRPCCall()
    {
        isMonsterTurn = true;
        photonView.RPC("MonsterTurnStart", RpcTarget.All);
    }

    [PunRPC]
    public void MonsterTurnStart()
    {
        print("몬스터 턴 시작");
        MonsterAction();
    }
    public void MonsterAction()
    {
        print("몬스터 행동 시작");
        BattleManagerCopy.Instance.enemyAnim.SetTrigger("Rage");
        StartCoroutine(MonsterTurnEnd());
    }

    public IEnumerator MonsterTurnEnd()
    {
        print("몬스터 행동 종료, 턴을 넘깁니다.");
        yield return new WaitForSeconds(2f);
        isMonsterTurn = false;
        EndTurn();
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
