using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public BattleManager battleManager;

    [Header("플레이어 턴 관련 목록")]
    private int playerCount;                      // 플레이어 수
    private bool[] turnComplete;                  // 각 플레이어의 턴 완료 여부
    private int[] selectionValue;                 // 각 플레이어의 선택지 값 (0: 미선택, 1: 공격, 2: 방어)

    [Header("UI 선택지")]
    public Button attackButton;                   // 공격 버튼
    public Button defendButton;                   // 방어 버튼
    public Button turnCompleteButton;             // 턴 완료 버튼

    [Header("UI 턴 표시")]
    public TMP_Text[] playerSelections;           // 각 플레이어의 상태를 표시할 배열
    public GameObject TurnComUI;                  // 주사위 굴리기 알림창

    private int currentPlayerIndex = 0;           // 현재 플레이어 인덱스
    private bool isTurnComplete = false;          // 현재 플레이어의 턴 완료 여부
    private int turnCount = 1;                    // 현재 턴 수

    private void Start()
    {
        battleManager = GetComponent<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("BattleManager가 할당되지 않았습니다.");
            return;
        }
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // 배열 초기화
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
        Debug.Log($"플레이어 순서 {currentPlayerIndex + 1}번의 공격 선택");
        selectionValue[currentPlayerIndex] = 1;  // 선택지 값 1로 설정 (1: 공격)
        SelectionUI(currentPlayerIndex, $"플레이어 {currentPlayerIndex + 1}", false);
    }

    private void OnClickDefend()
    {
        Debug.Log($"플레이어 순서 {currentPlayerIndex + 1}번의 방어 선택");
        selectionValue[currentPlayerIndex] = 2;  // 선택지 값 2로 설정 (2: 방어)
        SelectionUI(currentPlayerIndex, $"플레이어 {currentPlayerIndex + 1}", false);
    }

    private void OnClickTurnComplete()
    {
        Debug.Log($"플레이어 순서 {currentPlayerIndex + 1} 번이 선택 완료");
        turnComplete[currentPlayerIndex] = true;
        isTurnComplete = true;

        // UI에 선택 완료 상태 업데이트
        SelectionUI(currentPlayerIndex, $"플레이어 {currentPlayerIndex + 1}", true);

        // 다음 플레이어로 턴 넘기기
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerCount)
        {
            TurnComUI.SetActive(true);
            Debug.Log("모든 플레이어의 선택이 완료되었습니다.");
            EndTurn();
        }
    }

    private void EndTurn()
    {
        Debug.Log("전원 선택 완료. 전투 준비 중...");

        // 턴 수에 따라 배틀애니 호출
        switch (turnCount)
        {
            case 1:
                BattleAni.instance.Turn01(); // 1턴 진행
                break;
            case 2:
                BattleAni.instance.Turn02(); // 2턴 진행
                break;
            case 3:
                BattleAni.instance.Turn03(); // 3턴 진행
                break;
            default:
                Debug.Log("모든 턴이 완료되었습니다.");
                break;
        }

        // 턴 수 증가
        turnCount++;

        // 턴 초기화 및 다음 턴 준비
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

    private void SelectionUI(int playerIndex, string playerName, bool isComplete)
    {
        if (playerIndex < 0 || playerIndex >= playerSelections.Length)
        {
            Debug.LogError($"플레이어 인덱스 {playerIndex}가 playerSelections 배열 범위를 벗어났습니다.");
            return;
        }

        string action = selectionValue[playerIndex] == 1 ? "<b>공격</b>" : "<b>방어</b>";
        string selectionText = isComplete
            ? $"{playerName} 선택 완료: {action}"
            : $"{playerName} 선택 진행 중: {action}";
        playerSelections[playerIndex].text = selectionText;
    }
}
