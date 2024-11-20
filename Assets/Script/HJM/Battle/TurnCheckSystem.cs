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
        // Photon 네트워크가 연결되면 총 플레이어 수를 업데이트합니다.
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("현재 플레이어 수: " + totalPlayers);

        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);

        // 마스터 클라이언트만 게임 시작
        if (PhotonNetwork.IsMasterClient)
        {
            // 모든 플레이어가 입장하면 게임을 시작하도록 함
            if (totalPlayers >= 2) // 플레이어가 최소 2명이 되어야 게임을 시작하도록
            {
                StartGame();
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새로운 플레이어가 들어왔을 때 totalPlayers를 갱신
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("새로운 플레이어 입장: " + newPlayer.NickName);
        Debug.Log("현재 플레이어 수: " + totalPlayers);

        // 모든 플레이어가 입장한 후 마스터 클라이언트가 게임을 시작하도록 처리
        if (PhotonNetwork.IsMasterClient && totalPlayers >= 2)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);
    }

    void Update()
    {
        // 내 차례일 때만 버튼 활성화
        if (isMyTurn)
        {
            EnableBatUI();
            SetActiveTrueBatUI();
        }
        else
        {
            DisableBatUI();
            SetActiveFalseBatUI();
        }
    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
        // 현재 턴 설정
        currentTurnIndex = turnIndex;
        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];

        isMyTurn = currentPlayer == PhotonNetwork.LocalPlayer;

        if (isMyTurn)
        {
            Debug.Log("내 턴");
        }
        else
        {
            Debug.Log("다음 사람 턴");
        }
    }

    public void EndTurn()
    {
        if (!isMyTurn) return;

        // 턴을 넘길 때는 내 턴일때만 
        if (isMyTurn)
        {
            currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;
            photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
        }
    }

    public void OnClickAttack()
    {
        diceDamage = DiceRollManager.Get().BattleDiceRoll(3); // 보정치(유저의 힘 스탯 값) 임의값
        Debug.Log("주사위 굴린 결과: " + diceDamage);

        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);

        StartCoroutine(WaitAndEndTurn());
    }

    public void OnClickDefense()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);

        StartCoroutine(WaitAndEndTurn());
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(3f);

        EndTurn();
    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[playerIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
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
