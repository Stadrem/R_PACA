using System;
using UnityEngine;
using Object = UnityEngine.Object;


namespace UniversePlay
{
    public class NpcSpawner
    {
        public NpcInPlay Spawn(NpcInfo npc)
        {
            GameObject npcPrefab;
            switch (npc.Type)
            {
                case NpcInfo.ENPCType.None:
                    return null;
                case NpcInfo.ENPCType.Human:
                    npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Human");
                    break;
                case NpcInfo.ENPCType.Goblin:
                    npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Goblin");
                    break;
                case NpcInfo.ENPCType.Elf:
                    npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Elf");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var go = Object.Instantiate(npcPrefab, npc.Position, Quaternion.identity, null);
            var play = go.GetComponent<NpcInPlay>();
            play.Init(npc);
            return play;
        }
    }
}