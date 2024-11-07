using System;
using Photon.Pun;
using UnityEngine;


namespace UniversePlay
{
    public class NpcSpawner
    {
        public NpcInPlay PunSpawn(NpcInfo npc)
        {
            GameObject npcObject;
            switch (npc.Type)
            {
                case NpcInfo.ENPCType.None:
                    return null;
                case NpcInfo.ENPCType.Human:
                    npcObject = PhotonNetwork.Instantiate(
                        "BackgroundPart/NPC_Human",
                        npc.Position,
                        Quaternion.identity
                    );
                    break;
                case NpcInfo.ENPCType.Goblin:
                    npcObject = PhotonNetwork.Instantiate(
                        "BackgroundPart/NPC_Goblin",
                        npc.Position,
                        Quaternion.identity
                    );
                    break;
                case NpcInfo.ENPCType.Elf:
                    npcObject = PhotonNetwork.Instantiate(
                        "BackgroundPart/NPC_Elf",
                        npc.Position,
                        Quaternion.identity
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var play = npcObject.GetComponent<NpcInPlay>();
            play.Init(npc);
            return play;
        }
    }
}