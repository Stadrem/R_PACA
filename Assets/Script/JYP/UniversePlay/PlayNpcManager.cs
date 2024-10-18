using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayNpcManager : MonoBehaviour
{
    private List<NpcData> currentBackgroundNPCList = new();
    private List<NpcInPlay> currentNpcList = new();
    public CinemachineVirtualCamera CurrentNpcVcam => currentInteractNpc.ncVcam;
    private NpcInPlay currentInteractNpc;

    private TurnSystem turnSystem = new TurnSystem();

    public void LoadNpcList(List<NpcData> npcList)
    {
        if (currentNpcList.Count > 0)
        {
            currentNpcList.ForEach(Destroy);
            currentNpcList.Clear();
        }

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
                npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Goblin");
                break;
            case NpcData.ENPCType.Elf:
                npcPrefab = Resources.Load<GameObject>("BackgroundPart/NPC_Elf");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var go = Instantiate(npcPrefab, npc.Position, Quaternion.identity, null);
        var play = go.GetComponent<NpcInPlay>();
        play.Init(npc.Name);
        currentNpcList.Add(play);
    }

    public void InteractNpc(NpcInPlay npc)
    {
        currentInteractNpc = npc;
        StartCoroutine(PlayUserTurn());
    }

    IEnumerator PlayUserTurn()
    {
        bool res = false;
        int id = 0;
        yield return MockServer.Instance.Get<int>(
            (t) =>
            {
                res = true;
                id = t;
            }
        );

        yield return new WaitUntil(() => res);
        PlayUniverseManager.Instance.NpcChatManager.SetChattable(id == MyId);
    }

    public int MyId { get; } = 0;

    public void ChatNpc()
    {
    }
}