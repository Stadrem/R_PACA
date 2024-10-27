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
        play.Init(npc);
        currentNpcList.Add(play);
    }

    public void InteractNpc(NpcInPlay npc)
    {
        currentInteractNpc = npc;
        InitPlay();
        // photonView.RPC("InitPlay", RpcTarget.All);
        StartCoroutine(
            CheckCurrentTurnUser(
                (t =>
                    {
                        currentPlayerId = t;
                        NextTurn(0);
                        // photonView.RPC("NextTurn", RpcTarget.All, 0); // 0 for test
                    }
                )
            )
        );
    }

    public void OnChatSubmit(string msg)
    {
        StartCoroutine(ConversationWithNpc(PlayUniverseManager.Instance.InGamePlayerManager.MyInfo.name, msg));
    }

    //todo remove this prototype test code
    private int currentIdx = 0;

    private List<string> testMsgList = new List<string>
    {
        "노인:“반갑네 낯선 젊은이여“\n노인의 얼굴에는 근심과 걱정이 가득합니다. 노인은 플레이어의 모습을 관찰하고는 그가 모험가라는 사실을 깨닫습니다.\n노인:“마을에 큰 위협이 닥쳐왔네. 우리를 도와줄 수 있겠나.”\n노인은 플레이어의 대답을 기다립니다.",
        "노인:“고맙네 젊은이.“\n노인은 고개를 숙여 플레이어에게 감사의 인사를 전합니다. 노인은 지팡이로 마을의 끝을 가리킵니다.\n노인: “저 곳에 버려진 던전이 있다네. 그곳에 못된 고블린 하나가 우리 마을을 괴롭히고있다네.”\n노인은 슬픔이 가득한 눈으로 플레이어를 주시합니디. 노인은 외부인에게 도움을 요청해야하는 이 상황을 부끄럽게 생각하지만, 위기의 처한 마을에서는 별다른 방안이 없습니다.",
    };

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
                if (currentInteractNpc != null && currentInteractNpc.ShapeType == NpcData.ENPCType.Goblin)
                {
                    somethingToShow = "AdjklU jaklN!!";
                }
                else
                {
                    if (currentIdx >= testMsgList.Count)
                        currentIdx = 0;
                    somethingToShow = testMsgList[currentIdx++];
                }
            }
        );
        yield return new WaitForSeconds(1f);
        NpcChatUIManager.RPC_AddChatBubble(currentInteractNpc?.NpcName ?? "someting", somethingToShow);
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

        NextTurn(id);
        // photonView.RPC("NextTurn", RpcTarget.All, id);
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

    public void FinishConversation()
    {
        NpcChatUIManager.Hide();
    }

    public void ShowNpcHpBar()
    {
        Debug.Log($"currentInterACTnPC : {currentInteractNpc.name}");
        if (currentInteractNpc.root != null)
            currentInteractNpc.root.SetActive(true);
    }

    public void HideNpcHpBar()
    {
        if (currentInteractNpc.root != null)
            currentInteractNpc.root.SetActive(false);
    }
}