using System;
using Photon.Pun;
using UnityEngine;


namespace UniversePlay
{
    public class NpcSpawner
    {
        public NpcInPlay PunSpawn(NpcInfo npc)
        {
            
            const string backgroundPartNpcHuman = "BackgroundPart/NPC_Human";
            const string backgroundPartNpcGoblin = "BackgroundPart/NPC_Goblin";
            const string backgroundPartNpcElf = "BackgroundPart/NPC_Elf";
            const string backgroundPartNpcGolem = "BackgroundPart/NPC_Golem";
            
            GameObject npcObject;
            switch (npc.npcShapeType)
            {
                case NpcInfo.ENpcType.None:
                    return null;
                case NpcInfo.ENpcType.Human:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcHuman,
                        Vector3.zero,
                        Quaternion.identity,
                        group: 0,
                        data: new object[] {npc.id}
                    );
                    break;
                case NpcInfo.ENpcType.Goblin:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcGoblin,
                        Vector3.zero,
                        Quaternion.identity,
                        group: 0,
                        data: new object[] {npc.id}
                    );
                    break;
                case NpcInfo.ENpcType.Elf:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcElf,
                        Vector3.zero,
                        Quaternion.identity,
                        group: 0,
                        data: new object[] {npc.id}
                    );
                    break;
                
                case NpcInfo.ENpcType.Golem:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcGolem,
                        Vector3.zero,
                        Quaternion.identity,
                        group: 0,
                        data: new object[] {npc.id}
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