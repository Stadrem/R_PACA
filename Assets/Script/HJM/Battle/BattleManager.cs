using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> players;            // 플레이어 목록
    public List<NavMeshAgent> agents;           // NavMesh 에이전트 목록
    public List<PlayerMove> playerMoves;        // PlayerMove 스크립트 목록
    public List<Transform> battlePos;           // 전투 시 이동 위치
    public GameObject npcPrefab;                // NPC 프리팹
    public Transform npcPos;                    // NPC 생성 위치

    private bool[] turnCompleteStatus;          // 각 플레이어의 턴 완료 여부
    private int playerCount;                    // 플레이어 수

    private void Start()
    {
        playerCount = players.Count;
        turnCompleteStatus = new bool[playerCount];  // 플레이어 수만큼 턴 완료 여부 저장

        // 플레이어 목록에 추가되어야함

        // 플레이어당 NavMeshAgent와 PlayerMove 스크립트를 할당
        for (int i = 0; i < playerCount; i++)
        {
            agents[i] = players[i].GetComponent<NavMeshAgent>();
            playerMoves[i] = players[i].GetComponent<PlayerMove>();
        }
    }

    void Update()
    {
        // B키를 눌렀을 때 전투환경 세팅 (전투 시작 트리거 서버 통신으로 나중에 변경)
        if (Input.GetKey(KeyCode.B))
        {
            // NPC 생성
            Instantiate(npcPrefab, npcPos);

            // 모든 플레이어를 전투 위치로 이동시키고 이동 불가 설정
            for (int i = 0; i < playerCount; i++)
            {
                agents[i].enabled = false;                // 에이전트 비활성화
                playerMoves[i].clickMovementEnabled = false; // 클릭 이동 비활성화
                players[i].transform.position = battlePos[i].position;   // 전투 위치로 이동
                players[i].transform.rotation = battlePos[i].rotation;   // 전투 회전 방향으로 회전
            }
        }
    }

    
}
