using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayNpcManager : MonoBehaviourPun
{
    private List<NpcData> currentBackgroundNPCList = new();
    private List<NpcInPlay> currentNpcList = new();
    public CinemachineVirtualCamera CurrentNpcVcam => currentInteractNpc.ncVcam;
    private NpcInPlay currentInteractNpc;

    private TurnSystem turnSystem = new TurnSystem();
    private NpcChatUIManager NpcChatUIManager => PlayUniverseManager.Instance.NpcChatUIManager;
    private int currentPlayerId = -1;
    private bool isFinished = false;

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
        photonView.RPC("InitPlay", RpcTarget.All);
        StartCoroutine(
            CheckCurrentTurnUser(
                (t =>
                    {
                        currentPlayerId = t;
                        photonView.RPC("NextTurn", RpcTarget.All, 0); // 0 for test
                    }
                )
            )
        );
    }

    public void OnChatSubmit(string msg)
    {
        StartCoroutine(ConversationWithNpc(PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.name, msg));
    }

    private IEnumerator ConversationWithNpc(string sender, string message)
    {
        //sync with players

        bool wait = true;
        yield return MockServer.Instance.Get<string>(
            (t) =>
            {
                wait = false;
                Debug.Log(t);
            }
        );

        yield return new WaitWhile(() => wait);
        bool testDiceReq = true;
        if (testDiceReq)
        {
            bool waitRes = true;
            bool waitRoll = true;
            yield return MockServer.Instance.Get<int>(
                (t) => { waitRes = false; }
            );
            //roll dice
            waitRoll = false;
            string something = "something with dice";
            
            yield return new WaitWhile(() => waitRes || waitRoll);
        }
        else
        {
        }

        string somethingToShow = "";
        yield return MockServer.Instance.Get<string>(
            (t) =>
            {
                somethingToShow = "somethingToShow";
            }
        );
        yield return new WaitForSeconds(1f);
        NpcChatUIManager.RPC_AddChatBubble(sender, somethingToShow);
        //next turn
        bool res = false;
        int id = 0;
        
        yield return CheckCurrentTurnUser(
            (t) =>
            {
                res = true;
                id = 0;
            }
        );
        
        yield return new WaitUntil(() => res);
        
        photonView.RPC("NextTurn", RpcTarget.All, id);
    }

    IEnumerator CheckCurrentTurnUser(Action<int> callback)
    {
        yield return MockServer.Instance.Get<int>(
            (t) => { callback(t); }
        );
    }

    [PunRPC]
    private void NextTurn(int id)
    {
        turnSystem.NextTurn(id);
        PlayUniverseManager.Instance.NpcChatUIManager.SetTurnText(turnSystem.Turn, "");
        PlayUniverseManager.Instance.NpcChatUIManager.SetChattable(
            id == PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.id
        );
    }
    
    [PunRPC]
    private void InitPlay()
    {
        turnSystem.InitTurn();
        PlayUniverseManager.Instance.NpcChatUIManager.SetTurnText(turnSystem.Turn, "");
        
    }
}