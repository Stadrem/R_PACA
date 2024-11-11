using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem Instance { get; private set; }

    public int currentTurnIndex = 0;
    public int totalPlayers;
    public bool isMyTurn = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        photonView.RPC("BeginTurn", RpcTarget.AllBuffered, currentTurnIndex);
    }

    [PunRPC]
    void BeginTurn(int turnIndex)
    {
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

        isMyTurn = false;
        currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;

        photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
    }
}