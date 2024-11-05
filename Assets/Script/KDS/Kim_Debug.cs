using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kim_Debug : MonoBehaviourPunCallbacks
{
    //싱글톤
    public static Kim_Debug instance;

    public static Kim_Debug Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Kim_Debug");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<Kim_Debug>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
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

    public GameObject debugPanel;

    //백엔드 없을 시 디버그용
    void DebugPanel()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugPanel();
        }
    }

    public void OnClickNotNetwork()
    {
        TempFakeServer.Get();
    }

    public void OnClickDice()
    {
        DiceRollManager.Get().SearchDiceRoll(9);
    }

    public void OnClickBattleDice()
    {
        DiceRollManager.Get().BattleDiceRoll(9);
    }

    public void OnClickEnding()
    {
        Ending.Get().EnableCanvas();
    }

    public void OnClickLobby()
    {
        PhotonNetwork.LeaveRoom();

        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnClickTest()
    {
        var portalList1 = new List<PortalData>()
                {
                    new PortalData()
                    {
                        position = new Vector3(58.4300003f,9.30700016f,51.8300018f),
                        targetBackgroundId = 1,
                    },
                };

        var portalList2 = new List<PortalData>()
                {
                    new PortalData()
                    {
                        position = Vector3.zero,
                        targetBackgroundId = 0,
                    },
                };

        var npcList1 = new List<NpcInfo>()
                {
                    new NpcInfo()
                    {
                        Name = "마을사람 1",
                        Position = new Vector3(58.8600006f,9.57999992f,65.8899994f),
                        Type = NpcInfo.ENPCType.Human,
                    },

                    new NpcInfo()
                    {
                        Name = "고블린 1",
                        Position = new Vector3(62.4500008f,9.45199966f,66.5199966f),
                        Type = NpcInfo.ENPCType.Goblin,
                    }
                };
        var backgroundList = new List<BackgroundPartInfo>()
                {
                    new BackgroundPartInfo()
                    {
                        ID = 0,
                        Name = "Town 0",
                        Type = EBackgroundPartType.Town,
                        UniverseId = 0,
                        PortalList = portalList1,
                        NpcList = npcList1,
                    },
                    new BackgroundPartInfo()
                    {
                        ID = 1,
                        Name = "Dungeon 0",
                        Type = EBackgroundPartType.Dungeon,
                        UniverseId = 0,
                        PortalList = portalList2,
                        NpcList = new List<NpcInfo>()
                    },
                };

        var universe = new UniverseData()
        {
            id = 0,
            name = "Universe 0",
            startBackground = backgroundList[0],
        };
        PlayUniverseManager.Instance.InGamePlayerManager.Init();
        PlayUniverseManager.Instance.BackgroundManager.Init(universe, backgroundList);
    }
}

