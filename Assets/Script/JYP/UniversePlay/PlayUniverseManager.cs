﻿using System;
using System.ComponentModel;
using System.Linq;
using Data.Remote.Api;
using Photon.Pun;
using UI.UniversePlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UniversePlay;
using Utils;
using ViewModels;

/// <summary>
/// 플레이 화면에서의 전체적인 매니저
/// 모든 매니저들을 관리하고, 유저의 상호작용을 처리하는 클래스
/// </summary>
public class PlayUniverseManager : MonoBehaviourPun, IDisposable
{
    private static PlayUniverseManager instance;

    [SerializeField]
    private PlayBackgroundManager playBackgroundManager;

    public PlayBackgroundManager BackgroundManager => playBackgroundManager;

    [FormerlySerializedAs("playNpcManager")]
    [SerializeField]
    private InGameNpcManager inGameNpcManager;

    public InGameNpcManager NpcManager => inGameNpcManager;

    [SerializeField]
    private NpcChatUIManager npcChatUIManager;

    public NpcChatUIManager NpcChatUIManager => npcChatUIManager;

    [SerializeField]
    private CamSettingStateManager camSettingManager;

    public CamSettingStateManager CamSettingManager => camSettingManager;

    [SerializeField]
    private InGamePlayerManager inGamePlayerManager;

    public InGamePlayerManager InGamePlayerManager => inGamePlayerManager;

    [SerializeField]
    private PunSelectorChat selectorChat;

    [SerializeField]
    private GameObject hudUI;

    [SerializeField]
    private GameObject backgroundNameUI;

    [SerializeField]
    private Canvas canvas;
    
    [SerializeField]
    private IntroUIController introUIController;
    
    public bool IsHudVisible
    {
        get => hudUI.activeSelf;
        set
        {
            Debug.Log($"HUD Visible : {value}");
            hudUI.SetActive(value);
        }
    }
    
    public ISelectorChat SelectorChat => selectorChat;

    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

    public static PlayUniverseManager Instance => instance;

    public int universeId;
    public int roomNumber;
    public bool isBattle = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var code = Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties[PunPropertyNames.Room.ScenarioCode]);
        PhotonNetwork.AutomaticallySyncScene = true;
        ViewModel.PropertyChanged += OnPropertyChange;
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(ViewModel.LoadUniverseData(code));
    }

    private void OnPropertyChange(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.UniverseData))
        {
            if(ViewModel.UniverseData != null && photonView.IsMine)
            {
                universeId = ViewModel.UniverseData.id;
                photonView.RPC(nameof(Pun_SetUniverseId), RpcTarget.AllBuffered, universeId);
            }
        }
    }
    
    [PunRPC]
    private void Pun_SetUniverseId(int nextUniverseId)
    {
        universeId = nextUniverseId;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(GameMasterApi.Test(roomNumber, (res) => { }));
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                DisconnectToServer();
            }
        }

        UserInteraction();
    }

    private void DisconnectToServer()
    {
        GameMasterServerEventManager.Instance.Disconnect();

        StartCoroutine(
            PlayRoomApi.FinishRoom(
                roomNumber,
                (res) =>
                {
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadScene("LobbyScene");
                    Dispose();
                    GameMasterServerEventManager.Instance.DeleteInstance();
                }
            )
        );

        
    }


    private void UserInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue))
            {
                if (hit.collider.CompareTag("InPlayNPC") && !isBattle)
                {
                    NpcInteract(hit);
                }
                else if (hit.collider.CompareTag("Portal"))
                {
                    hit.collider.GetComponent<PortalInPlay>()
                        ?.InteractByUser();
                }
            }
        }
    }


    private void NpcInteract(RaycastHit hit)
    {
        var npc = hit.collider.GetComponent<InGameNpc>();
        if (npc == null) return;
        photonView.RPC(nameof(NpcInteract), RpcTarget.All, npc.NpcId);
    }

    [PunRPC]
    private void NpcInteract(int npcId)
    {
        inGameNpcManager.InteractNpc(npcId);
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.TalkView);
    }


    public void FinishConversation(bool endAiTalk = true)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (endAiTalk)
        {
            StartCoroutine(
                PlayProgressApi.FinishNpcTalk(
                    roomNumber,
                    (res) => { photonView.RPC(nameof(RPC_FinishConversation), RpcTarget.All); }
                )
            );
        }
        else
        {
            photonView.RPC(nameof(RPC_FinishConversation), RpcTarget.All);
        }
        
    }

    [PunRPC]
    public void RPC_FinishConversation()
    {
        NpcManager.FinishConversation();
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.QuarterView);
        SelectorChat.ClearOptions();
    }

    public void StartPlay()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Alert.Get().Set("맵 입장중...", 2.0f);
        LoadingManager.Instance.StartLoading();
        roomNumber = PhotonNetwork.CurrentRoom.Name.GetHashCode();
        var codeList = ViewModel.UniversePlayers
            .Select((t) => t.UserCode)
            .ToList();
        StartCoroutine(
            ViewModel.StartRoom(
                roomNumber,
                PhotonNetwork.CurrentRoom.Name,
                codeList,
                (res) =>
                {
                    if (res.IsSuccess)
                    {
                        BackgroundManager.StartFirstBackground();
                    }
                    else
                    {
                        LoadingManager.Instance.FinishLoading();
                    }
                }
            )
        );
        GameMasterServerEventManager.Instance.Connect(roomNumber);
    }

    public static void Create()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }

        var go = PhotonNetwork.Instantiate(
            "UniversePlay/UniversePlayManager",
            Vector3.zero,
            Quaternion.identity
        );
    }

    private void OnDestroy()
    {
        ViewModel.PropertyChanged -= OnPropertyChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnApplicationQuit()
    {
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(
                    PlayRoomApi.FinishRoom(
                        roomNumber,
                        (res) => { }
                    )
                );
            }
        }
    }

    public void Dispose()
    {
        ViewModelManager.Instance.Reset();
        instance = null;
        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnSceneLoaded");
        if(scene.name == "LobbyScene")
        {
            return;
        }
        else
        if (scene.name == "WaitingScene")
        {
            OnWaitingSceneLoaded();
        }
        else
        {
            OnPlaySceneLoaded();
        }
    }

    private void OnPlaySceneLoaded()
    {
        if (!IsHudVisible)
            IsHudVisible = true;
        var background =
            ViewModel.UniverseData.backgroundPartDataList.Find((t) => t.ID == ViewModel.CurrentBackgroundId);

        InGamePlayerManager.SpawnPlayers();
        NpcManager.LoadNpcList(background.NpcList);
        var go = Instantiate(backgroundNameUI, canvas.transform);
        var controller = go.GetComponent<BackgroundNameDisplayUIController>();
        controller.ShowBackgroundName(background.Name);
    }

    private void OnWaitingSceneLoaded()
    {
        if (IsHudVisible)
            IsHudVisible = false;
    }

    public void SetIntro(string introMessage)
    {
        photonView.RPC(nameof(Pun_SetIntro), RpcTarget.All, introMessage);
    }
    

    [PunRPC]
    private void Pun_SetIntro(string introMessage)
    {
        introUIController.SetIntroText(introMessage);
        introUIController = null;
    }
}