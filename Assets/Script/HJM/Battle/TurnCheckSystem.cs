using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public BattleManager battleManager;

    [Header("플레이어 턴 관련 목록")]
    public List<GameObject> players;              // 플레이어 목록
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



    private void Start()
    {
        battleManager = GetComponent<BattleManager>();
        if (battleManager != null)
        {
            players = battleManager.players;
            playerCount = players.Count;
            turnComplete = new bool[playerCount];
            selectionValue = new int[playerCount];  // 선택지 값 초기화
        }
        else
        {
            Debug.LogError("BattleManager가 할당되지 않았습니다.");
        }

        attackButton.onClick.AddListener(OnClickAttack);
        defendButton.onClick.AddListener(OnClickDefend);
        turnCompleteButton.onClick.AddListener(OnClickTurnComplete);

        // 턴 초기화
        ResetTurn();
    }

    public void StartTurn()
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
        turnComplete[currentPlayerIndex] = true;    // 현재 플레이어 턴 완료
        isTurnComplete = true;                      // 현재 플레이어의 턴을 완료로 표시

        // UI에 선택 완료 상태 업데이트
        SelectionUI(currentPlayerIndex, $"플레이어 {currentPlayerIndex + 1}", true);

        TurnComUI.SetActive(true);
        // 다음 플레이어로 턴 넘기기
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerCount)
        {
            Debug.Log("모든 플레이어의 선택이 완료되었습니다.");
            EndTurn();
        }
    }

    // 모든 플레이어가 턴을 완료했을 시
    private void EndTurn()
    {
        Debug.Log("전원 선택 완료. 전투 준비 중...");
        SendSelectionsToServer();
        StartTurn();
    }

    private void SendSelectionsToServer()
    {
        for (int i = 0; i < playerCount; i++)
        {
            Debug.Log($"플레이어 {i + 1}의 선택은: {(selectionValue[i] == 1 ? "공격" : "방어")}");
        }
    }

    // 턴 초기화
    private void ResetTurn()
    {
        for (int i = 0; i < playerCount; i++)
        {
            turnComplete[i] = false;
            selectionValue[i] = 0;
        }
    }

    // 선택 완료 체크
    private bool AllTurnsComplete()
    {
        foreach (bool completed in turnComplete)
        {
            if (!completed) return false;
        }
        return true;
    }

    private void SendTurnComplete(int playerIndex, int selection)
    {
        selectionValue[playerIndex] = selection;
        turnComplete[playerIndex] = true;
        Debug.Log($"플레이어 {playerIndex + 1}의 선택을 동기화했습니다: {(selection == 1 ? "공격" : "방어")}");
    }

    // 해당 플레이어의 선택지 UI 업데이트
    private void SelectionUI(int playerIndex, string playerName, bool isComplete)
    {
        string action = selectionValue[playerIndex] == 1 ? "<b>공격</b>" : "<b>방어</b>";
        string selectionText = isComplete
            ? $"{playerName} 선택 완료: {action}"
            : $"{playerName} 선택 진행 중: {action}";
        playerSelections[playerIndex].text = selectionText;
    }
}
