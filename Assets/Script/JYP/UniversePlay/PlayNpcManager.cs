using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayNpcManager : MonoBehaviourPun
{
    private List<NpcData> currentBackgroundNPCList = new();
    private List<NpcInPlay> currentNpcList = new();
    public CinemachineVirtualCamera CurrentNpcVcam => currentInteractNpc.ncVcam;
    private NpcInPlay currentInteractNpc;

    private TurnSystem turnSystem = new TurnSystem();
    private NpcChatUIManager npcChatUIManager = PlayUniverseManager.Instance.NpcChatUIManager;


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

    public void OnChatSubmit(string msg)
    {
    }

    private IEnumerator ConversationWithNpc(string sender, string message)
    {
        bool wait = true;
        yield return MockServer.Instance.Get<string>(
            (t) =>
            {
                wait = false;
                Debug.Log(t);
            }
        );

        yield return new WaitWhile(() => wait);
        bool testDiceReq = false;
        if (testDiceReq)
        {
            yield return MockServer.Instance.Get<int>(
                (t) => { }
            );
        }
        else
        {
            
        }
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
        PlayUniverseManager.Instance.NpcChatUIManager.SetChattable(id == MyId);
    }

    public int MyId { get; } = 0;

    public void ChatToNPC(string msg)
    {
        //send to backend
        photonView.RPC("MessageSend", RpcTarget.All, MyId, msg);
    }

    [PunRPC]
    private void MessageSend(int senderId, string message)
    {
    }
}