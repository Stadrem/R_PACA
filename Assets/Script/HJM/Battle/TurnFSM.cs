using Photon.Pun;
using UnityEngine;
using System.Collections;

public enum ActionTurn
{
    Base = -1,
    Player = 0,
    Enemy = 1,
}

public class TurnFSM : MonoBehaviourPunCallbacks
{
    public static TurnFSM Instance { get; private set; }

    public ActionTurn turnState = ActionTurn.Player;
    public int finishActionCount = 0; // 플레이어들의 행동 완료 누적 카운트
    private int totalPlayers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    
    public void FinishPlayerTurn()
    {
        if (turnState != ActionTurn.Player) return;

        finishActionCount++;
        if (finishActionCount >= totalPlayers)
        {
            finishActionCountOver(); // 모든 플레이어가 턴을 종료했을 때
        }
    }

    
    private void finishActionCountOver()
    {
        photonView.RPC("ChangeTurnToEnemy", RpcTarget.All);
        finishActionCount = 0; // 카운트 초기화
    }

    [PunRPC]
    public void ChangeTurnToEnemy()
    {
        turnState = ActionTurn.Enemy;
        Debug.Log("몬스터 턴 시작");
        MonsterTurn();
    }

    private void MonsterTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트만 몬스터 행동을 처리
            StartCoroutine(MonsterAction());
        }
    }

    private IEnumerator MonsterAction()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("몬스터 행동 완료");

        photonView.RPC("ChangeTurnToPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeTurnToPlayer()
    {
        turnState = ActionTurn.Player;
        Debug.Log("플레이어 턴 시작");
    }


}
