using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.JYP.UniversePlay
{
    public class UniversePlayTest : MonoBehaviour
    {
        private void Start()
        {
            var instance = PlayUniverseManager.Instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayUniverseManager.Instance.FinishConversation();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
            }
            else TestingByKey();
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

                var npcList1 = new List<NpcData>()
                {
                    new NpcData()
                    {
                        Name = "마을사람 1",
                        Position = new Vector3(58.8600006f,9.57999992f,65.8899994f),
                        Type = NpcData.ENPCType.Human,
                    },

                    new NpcData()
                    {
                        Name = "고블린 1",
                        Position = new Vector3(62.4500008f,9.45199966f,66.5199966f),
                        Type = NpcData.ENPCType.Goblin,
                    }
                };
                var backgroundList = new List<BackgroundPartInfo>()
                {
                    new BackgroundPartInfo()
                    {
                        id = 0,
                        Name = "Town 0",
                        Type = EBackgroundPartType.Town,
                        universeId = 0,
                        portalList = portalList1,
                        npcList = npcList1,
                    },
                    new BackgroundPartInfo()
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
                PlayUniverseManager.Instance.InGamePlayerManager.Init();
                PlayUniverseManager.Instance.BackgroundManager.Init(universe, backgroundList);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayUniverseManager.Instance.BackgroundManager.MoveTo(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayUniverseManager.Instance.BackgroundManager.MoveTo(1);
            }
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

            var npcList1 = new List<NpcData>()
                {
                    new NpcData()
                    {
                        Name = "마을사람 1",
                        Position = new Vector3(58.8600006f,9.57999992f,65.8899994f),
                        Type = NpcData.ENPCType.Human,
                    },

                    new NpcData()
                    {
                        Name = "고블린 1",
                        Position = new Vector3(62.4500008f,9.45199966f,66.5199966f),
                        Type = NpcData.ENPCType.Goblin,
                    }
                };
            var backgroundList = new List<BackgroundPartInfo>()
                {
                    new BackgroundPartInfo()
                    {
                        id = 0,
                        Name = "Town 0",
                        Type = EBackgroundPartType.Town,
                        universeId = 0,
                        portalList = portalList1,
                        npcList = npcList1,
                    },
                    new BackgroundPartInfo()
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
            PlayUniverseManager.Instance.InGamePlayerManager.Init();
            PlayUniverseManager.Instance.BackgroundManager.Init(universe, backgroundList);
        }

        #endregion
    }
}