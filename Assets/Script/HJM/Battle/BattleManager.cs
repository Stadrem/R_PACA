using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleManager : MonoBehaviour
{
    [Header("플레이어 관련 목록")]
    public List<GameObject> players;            // 플레이어 목록
    public List<Transform> battlePos;           // 전투 시 이동 위치
    public List<NavMeshAgent> agents;           // NavMesh 에이전트
    public List<PlayerMove> playerMoves;        // PlayerMove 스크립트
    public GameObject playerPrefab;             // 플레이어 프리팹

    [Header("NPC 생성 옵션")]
    public GameObject npcPrefab;                // NPC 프리팹
    public Transform npcPos;                    // NPC 생성 위치
    public int npcCount = 1;                    // 생성할 NPC 수

    private bool[] turnComplete;                // 각 플레이어의 턴 완료 여부
    private int playerCount = 1;                // 플레이어 수 (싱글 플레이용으로 1로 고정)
    private int currentNpcCount = 0;            // 현재 생성된 NPC 수

    public GameObject battleUI;                 // 전투UI

    private void Start()
    {

        if (players == null || players.Count == 0)
        {
            players = new List<GameObject>();
        }
        // 태그 Player 찾아서 players 리스트에 추가
        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(playersArray);


        
        if (players.Count > 0)
        {
            // NavMeshAgent와 PlayerMove 스크립트 할당
            agents.Add(players[0].GetComponent<NavMeshAgent>());
            playerMoves.Add(players[0].GetComponent<PlayerMove>());
        }
        else
        {
            print("플레이어가 할당되지 않았습니다.");
        }

    }

    void Update()
    {
        if (players == null || players.Count == 0)
        {
            players = new List<GameObject>(); 
                                              
            GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
            players.AddRange(playersArray);
            
            if (players.Count > 0)
            {
                agents.Add(players[0].GetComponent<NavMeshAgent>());
                playerMoves.Add(players[0].GetComponent<PlayerMove>());
            }
        }

        // B키를 눌렀을 때 전투환경 세팅
        if (Input.GetKey(KeyCode.B))
        {
            // 생성할 NPC의 수만큼 생성
            if (currentNpcCount < npcCount)
            {
                SpawnNPCs();
            }

            //PlayUniverseManager.Instance.ShowBattleUI();
            //PlayUniverseManager.Instance.FinishConversation();
            // 플레이어 전투 위치로 이동
            MoveToBattlePos();
            battleUI.SetActive(true);

        }
    }

    // NPC 생성
    public void SpawnNPCs()
    {
        if (currentNpcCount < npcCount)
        {
            GameObject npc = Instantiate(npcPrefab, npcPos.position, npcPos.rotation);
            currentNpcCount++;
        }
    }

    // 플레이어를 전투 위치로 이동
    public void MoveToBattlePos()
    {

        agents[0].enabled = false;
        playerMoves[0].clickMovementEnabled = false;
        players[0].transform.position = battlePos[0].position;
        players[0].transform.rotation = battlePos[0].rotation;

    }
}
