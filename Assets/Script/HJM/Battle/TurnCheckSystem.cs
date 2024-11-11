using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.Collections;

public class TurnCheckSystem : MonoBehaviourPunCallbacks
{
    public static TurnCheckSystem Instance { get; private set; }
    public static CircularSlider circularSlider;

    [Header("턴 순환")]
    public int currentTurnIndex = 0;
    public int totalPlayers;
    public bool isMyTurn = false;

    [Header("턴 UI")]
    public Button attackBtn;
    public Button defenseBtn;
    public Button turnCompBtn;
    public GameObject turnSilder;


    public bool isMyTurnAction = false;

    public List<GameObject> profiles;

    //[Header("선택지 UI")]
    //public List<Image> selectImgs;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        circularSlider = turnSilder.GetComponent<CircularSlider>();
        attackBtn.onClick.AddListener(OnClickAttack);
        defenseBtn.onClick.AddListener(OnClickDefense);


        totalPlayers = PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    void Update()
    {
        if (isMyTurn && isMyTurnAction == true)
        {
            PerformAction();
        }
    }
    private void PerformAction()
    {
        Debug.Log("턴 선택 행동 ~~");

        // 주사위 굴리기
        int result = DiceRollManager.Get().BattleDiceRoll(3); // 임의로 3
        print(result);

        // 굴리는거 기다림
        StartCoroutine(WaitAndEndTurn());
        
        Debug.Log("턴 선택 행동 끝");
        isMyTurnAction = false;
    }

    private IEnumerator WaitAndEndTurn()
    {
        yield return new WaitForSeconds(5f);
        // 안내창을 띄우자 . . . 나중에 좀 인터렉션을 넣을까

        EndTurn(); // 턴 종료(다른 플레이어에게 턴 넘어감)
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
            // 타이머 시작하기
            circularSlider.StartDepletion();
            // 선택지 버튼 활성화
            EnableButtons();
        }
        else
        {
            Debug.Log("다음 사람 턴");
            // 타이머 초기화하고 멈춰놓기
            circularSlider.ResetSlider();
            // 선택지 버튼 비 활성화
            DisableButtons();
        }
    }

    public void EndTurn()
    {
        if (!isMyTurn) return;

        isMyTurn = false;
        currentTurnIndex = (currentTurnIndex + 1) % totalPlayers;

        photonView.RPC("BeginTurn", RpcTarget.All, currentTurnIndex);
    }

    public void OnClickAttack()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 1);
    }

    public void OnClickDefense()
    {
        photonView.RPC("UpdateSelectImage", RpcTarget.All, currentTurnIndex, 2);
    }

    [PunRPC]
    public void UpdateSelectImage(int playerIndex, int selection)
    {
        profiles[currentTurnIndex].GetComponent<ProfileSet>().SetSelectImage(selection);
    }


    public void EnableButtons()
    {
        attackBtn.interactable = true;
        defenseBtn.interactable = true;
        turnCompBtn.interactable = true;
    }

    public void DisableButtons()
    {
        attackBtn.interactable = false;
        defenseBtn.interactable = false;
        turnCompBtn.interactable = false;
    }


}