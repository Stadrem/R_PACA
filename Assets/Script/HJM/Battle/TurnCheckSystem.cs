using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem instance;

    public BattleManager battleManager;
    public PlayerBatList playerBatList;
    //public CircularSlider circularSlider;

    [Header("플레이어 턴 관련 목록")]
    private int playerCount;
    private bool[] turnComplete;
    private int[] selectionValue;

    [Header("UI 선택지")]
    public Button attackButton;
    public Button defendButton;
    public Button turnCompleteButton;

    [Header("UI 턴 표시")]
    public TMP_Text[] playerSelections;

    public GameObject TurnComUI;
    public int turnCount = 1;
    private int currentPlayerIndex = 0;
    private bool isTurnComplete = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        battleManager = GetComponent<BattleManager>();
        playerBatList = GetComponent<PlayerBatList>();
        //circularSlider = GetComponentInChildren<CircularSlider>();

        if (PhotonNetwork.InRoom)
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            
        }

        playerBatList.GetBattlePlayers().Sort((a, b) => b.dexterity.CompareTo(a.dexterity));  // 속도 내림차순 정렬
        turnComplete = new bool[playerCount];
        selectionValue = new int[playerCount];

        attackButton.onClick.AddListener(OnClickAttack);
        defendButton.onClick.AddListener(OnClickDefend);
        turnCompleteButton.onClick.AddListener(OnClickTurnComplete);

        StartTurn();
    }

    private void StartTurn()
    {
        currentPlayerIndex = 0;
        ResetTurn();
        isTurnComplete = false;
    }

    private void OnClickAttack()
    {
        int sortedIndex = GetSortedPlayerIndex();
        selectionValue[sortedIndex] = 1;
        photonView.RPC("SelectionUI", RpcTarget.All, sortedIndex, $"플레이어 {playerBatList.GetBattlePlayers()[sortedIndex].nickname}", false);
    }

    private void OnClickDefend()
    {
        int sortedIndex = GetSortedPlayerIndex();
        selectionValue[sortedIndex] = 2;
        photonView.RPC("SelectionUI", RpcTarget.All, sortedIndex, $"플레이어 {playerBatList.GetBattlePlayers()[sortedIndex].nickname}", false);
    }

    private void OnClickTurnComplete()
    {
        int sortedIndex = GetSortedPlayerIndex();
        turnComplete[sortedIndex] = true;
        isTurnComplete = true;
        photonView.RPC("SelectionUI", RpcTarget.All, sortedIndex, $"플레이어 {playerBatList.GetBattlePlayers()[sortedIndex].nickname}", true);

        photonView.RPC("CheckAllPlayersTurnComplete", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void CheckAllPlayersTurnComplete()
    {
        bool allPlayersTurnComplete = true;
        for (int i = 0; i < playerCount; i++)
        {
            if (!turnComplete[i])
            {
                allPlayersTurnComplete = false;
                break;
            }
        }

        if (allPlayersTurnComplete)
        {
            photonView.RPC("EndTurn", RpcTarget.All);
        }
    }

    [PunRPC]
    private void EndTurn()
    {
        
        print("턴이 완료 되었습니다.");

        turnCount++;
        ResetTurn();
        StartTurn();
    }

    private void ResetTurn()
    {
        for (int i = 0; i < playerCount; i++)
        {
            turnComplete[i] = false;
            selectionValue[i] = 0;
        }
    }

    [PunRPC]
    private void SelectionUI(int playerIndex, string playerName, bool isComplete)
    {
        //if (playerIndex < 0 || playerIndex >= playerSelections.Length)
        //{
        //    Debug.LogError($"플레이어 인덱스 {playerIndex}가 playerSelections 배열 범위를 벗어났습니다.");
        //    return;
        //}

        string action = selectionValue[playerIndex] == 1 ? "<color=#FF0000><b>공격</b></color>" : "<color=#0000FF><b>방어</b></color>";
        string selectionText = isComplete
            ? $"{playerName} 선택 완료: {action}"
            : $"{playerName} 선택 진행 중: {action}";
        playerSelections[playerIndex].text = selectionText;
    }

    private int GetSortedPlayerIndex()
    {
        return playerBatList.GetBattlePlayers().FindIndex(p => p.nickname == PhotonNetwork.LocalPlayer.NickName);
    }
}
