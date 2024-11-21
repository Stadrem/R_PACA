using System;
using Data.Models.Universe.Characters;
using Photon.Pun;
using UnityEngine;


namespace UniversePlay
{
    public class NpcSpawner
    {
        public InGameNpc PunSpawn(UniverseNpc npc)
        {
            
            const string backgroundPartNpcHuman = "BackgroundPart/NPC_Human";
            const string backgroundPartNpcGoblin = "BackgroundPart/NPC_Goblin";
            const string backgroundPartNpcElf = "BackgroundPart/NPC_Elf";
            const string backgroundPartNpcGolem = "BackgroundPart/NPC_Golem";
            Debug.Log($"npc: {npc.Name} / {npc.NpcShapeType} / {npc.Position} / {npc.YRotation}");
            var rot = Quaternion.Euler(0, npc.YRotation, 0);
            GameObject npcObject;
            switch (npc.NpcShapeType)
            {
                case UniverseNpc.ENpcType.None:
                    return null;
                case UniverseNpc.ENpcType.Human:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcHuman,
                        Vector3.zero,
                        rot,
                        group: 0,
                        data: new object[] {npc.Id}
                    );
                    break;
                case UniverseNpc.ENpcType.Goblin:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcGoblin,
                        Vector3.zero,
                        rot,
                        group: 0,
                        data: new object[] {npc.Id}
                    );
                    break;
                case UniverseNpc.ENpcType.Elf:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcElf,
                        Vector3.zero,
                        rot,
                        group: 0,
                        data: new object[] {npc.Id}
                    );
                    break;
                
                case UniverseNpc.ENpcType.Golem:
                    npcObject = PhotonNetwork.Instantiate(
                        backgroundPartNpcGolem,
                        Vector3.zero,
                        rot,
                        group: 0,
                        data: new object[] {npc.Id}
                    );
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var play = npcObject.GetComponent<InGameNpc>();
            play.Init(npc);
            return play;
        }
    }
}