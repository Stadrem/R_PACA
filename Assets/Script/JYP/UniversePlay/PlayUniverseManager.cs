using System;
using System.ComponentModel;
using System.Linq;
using Data.Remote.Api;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniversePlay;
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

    [SerializeField]
    private PlayNpcManager playNpcManager;

    public PlayNpcManager NpcManager => playNpcManager;

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
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(
                    PlayRoomApi.FinishRoom(
                        roomNumber,
                        (res) =>
                        {
                            PhotonNetwork.LeaveRoom();
                            PhotonNetwork.LoadLevel("LobbyScene");
                            Dispose();
                        }
                    )
                );
            }
        }

        UserInteraction();
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
        var npc = hit.collider.GetComponent<NpcInPlay>();
        if (npc == null) return;
        photonView.RPC(nameof(NpcInteract), RpcTarget.All, npc.NpcId);
    }

    [PunRPC]
    private void NpcInteract(int npcId)
    {
        playNpcManager.InteractNpc(npcId);
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.TalkView);
    }


    public void FinishConversation()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        StartCoroutine(
            PlayProgressApi.FinishNpcTalk(
                roomNumber,
                (res) => { photonView.RPC(nameof(RPC_FinishConversation), RpcTarget.All); }
            )
        );
    }

    [PunRPC]
    public void RPC_FinishConversation()
    {
        NpcManager.FinishConversation();
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.QuarterView);
        SelectorChat.ClearOptions();
    }

    /// <summary>
    /// 전투화면에서의 UI를 보여주는 함수, 등장인물들의 HP UI를 보여준다.
    /// NPC의 경우, 대화를 시작했다는 가정하에 대화하고 있던 Npc의 HP UI를 보여준다.
    /// </summary>
    public void ShowBattleUI()
    {
        NpcManager.ShowNpcHpBar();
        InGamePlayerManager.ShowPlayersHpBar();
    }

    /// <summary>
    /// 전투화면에서의 UI를 숨기는 함수, 등장인물들의 HP UI를 숨긴다.
    /// </summary>
    public void HideBattleUI()
    {
        NpcManager.HideNpcHpBar();
        InGamePlayerManager.HidePlayersHpBar();
    }

    public void StartPlay()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        roomNumber = PhotonNetwork.CurrentRoom.Name.GetHashCode();
        var codeList = InGamePlayerManager.playerList
            .Select((t) => t.code)
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
                        InGamePlayerManager.Init();
                        BackgroundManager.StartFirstBackground();
                    }
                }
            )
        );
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
        instance = null;
        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnSceneLoaded");
        if (arg0.name == "WaitingScene")
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
}