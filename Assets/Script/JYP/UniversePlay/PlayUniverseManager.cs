using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayUniverseManager : MonoBehaviour
{
    private static PlayUniverseManager instance;

    [SerializeField]
    private PlayBackgroundManager playBackgroundManager;

    public PlayBackgroundManager BackgroundManager => playBackgroundManager;

    [FormerlySerializedAs("playNPCManager")]
    [SerializeField]
    private PlayNpcManager playNpcManager;

    public PlayNpcManager NpcManager => playNpcManager;

    [SerializeField]
    private NpcChatManager npcChatManager;
    public  NpcChatManager NpcChatManager => npcChatManager;
    
    [SerializeField]
    private CamSettingStateManager camSettingManager;
    public CamSettingStateManager CamSettingManager => camSettingManager;
    
    public List<int> testPlayerIdList = new List<int>()
    {
        0, 1, 2, 3
    };
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

    private void Update()
    {
        TestingByKey();
        UserInteraction();
    }

    private void UserInteraction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue))
            {
                if (hit.collider.CompareTag("InPlayNPC"))
                {
                    playNpcManager.InteractNpc(hit.collider.GetComponent<NpcInPlay>());
                    CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.TalkView);
                }
            }
        }
    }

    #region Test

    private void TestingByKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var portalList1 = new List<PortalData>()
            {
                new PortalData()
                {
                    position = new Vector3(0, 0, 0),
                    targetBackgroundId = 1,
                },
            };

            var portalList2 = new List<PortalData>()
            {
                new PortalData()
                {
                    position = new Vector3(0, 0, 0),
                    targetBackgroundId = 0,
                },
            };

            var npcList1 = new List<NpcData>()
            {
                new NpcData()
                {
                    Name = "마을사람 1",
                    Position = new Vector3(0, 0, 0),
                    Type = NpcData.ENPCType.Human,
                },

                new NpcData()
                {
                    Name = "고블린 1",
                    Position = new Vector3(2, 0, 0),
                    Type = NpcData.ENPCType.Goblin,
                }
            };
            var backgroundList = new List<BackgroundPartData>()
            {
                new BackgroundPartData()
                {
                    id = 0,
                    Name = "Town 0",
                    Type = EBackgroundPartType.Town,
                    universeId = 0,
                    portalList = portalList1,
                    npcList = npcList1,
                },
                new BackgroundPartData()
                {
                    id = 1,
                    Name = "Dungeon 0",
                    Type = EBackgroundPartType.Dungeon,
                    universeId = 0,
                    portalList = portalList2,
                    npcList = new List<NpcData>()
                },
            };

            var universe = new UniverseData()
            {
                id = 0,
                name = "Universe 0",
                startBackground = backgroundList[0],
            };

            playBackgroundManager.Init(universe, backgroundList);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playBackgroundManager.MoveTo(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playBackgroundManager.MoveTo(1);
        }
    }

    #endregion
}