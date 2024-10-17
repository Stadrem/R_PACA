using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayNpcManager : MonoBehaviour
{
    private List<NpcData> currentBackgroundNPCList;
    private List<NpcInPlay> currentNPCList;

    public void LoadNpcList(List<NpcData> npcList)
    {
        currentNPCList.ForEach(Destroy);
        currentNPCList.Clear();

        currentBackgroundNPCList = npcList;
        foreach (var npcData in currentBackgroundNPCList)
        {
            SpawnNPC(npcData);
        }
    }

    private void SpawnNPC(NpcData npc)
    {
        GameObject npcPrefab;
        switch (npc.Type)
        {
            case NpcData.ENPCType.None:
                return;
            case NpcData.ENPCType.Human:
                npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Human");
                break;
            case NpcData.ENPCType.Goblin:
                npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Gobiln");
                break;
            case NpcData.ENPCType.Elf:
                npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Elf");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var go = Instantiate(npcPrefab, npc.Position, Quaternion.identity, null);
        currentNPCList.Add(go.GetComponent<NpcInPlay>());
    }
}