using Photon.Pun;
using UnityEngine;

public class PlayUniverseManager : MonoBehaviour
{
    private static PlayUniverseManager instance;

    [SerializeField] private PlayBackgroundManager playBackgroundManager;

    public PlayBackgroundManager BackgroundManager => playBackgroundManager;

    [SerializeField] private PlayNpcManager playNpcManager;

    public PlayNpcManager NpcManager => playNpcManager;

    [SerializeField] private NpcChatUIManager npcChatUIManager;

    public NpcChatUIManager NpcChatUIManager => npcChatUIManager;

    [SerializeField] private CamSettingStateManager camSettingManager;

    public CamSettingStateManager CamSettingManager => camSettingManager;
    public InGamePlayerManager InGamePlayerManager { get; private set; }


    public static PlayUniverseManager Instance
    {
        get
        {
            if (instance == null)
            {
                var universePrefab = Resources.Load<GameObject>("UniversePlay/UniversePlayManager");
                var go = Instantiate(universePrefab);
            }

            return instance;
        }
    }

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
        InGamePlayerManager = GetComponent<InGamePlayerManager>();
    }

    private void Update()
    {
        UserInteraction();
    }

    private void UserInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue))
            {
                if (hit.collider.CompareTag("InPlayNPC"))
                {
                    playNpcManager.InteractNpc(hit.collider.GetComponent<NpcInPlay>());
                    CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.TalkView);
                }
                else if (hit.collider.CompareTag("Portal"))
                {
                    hit.collider.GetComponent<PortalInPlay>()
                        ?.InteractByUser();
                }
            }
        }
    }

    public void FinishConversation()
    {
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.QuarterView);
    }
}