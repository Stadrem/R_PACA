using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Data.Models.Universe.Characters;
using Data.Remote.Api;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;
using ViewModels;

namespace UniversePlay
{
    public class InGameNpcManager : MonoBehaviourPun
    {
        // private List<NpcInfo> currentBackgroundNPCList = new();

        public Transform npcSpawnOffset;

        private List<InGameNpc> currentNpcList = new();

        public CinemachineVirtualCamera CurrentNpcVcam => currentInteractInGameNpc.ncVcam;
        [FormerlySerializedAs("currentInteractNpc")]
        public InGameNpc currentInteractInGameNpc;
        private TurnSystem turnSystem = new();

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;
        private NpcChatUIManager NpcChatUIManager => PlayUniverseManager.Instance.NpcChatUIManager;
        private static InGamePlayerManager PlayerManager => PlayUniverseManager.Instance.InGamePlayerManager;
        private int currentPlayerId = -1;

        public ISelectorChat selectorChat => PlayUniverseManager.Instance.SelectorChat;

        private readonly NpcSpawner spawner = new NpcSpawner();


        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameMasterServerEventManager.Instance.OnEventReceived += (data) =>
            {
                photonView.RPC(nameof(RPC_ApplyGameMasterChatBubble), RpcTarget.All, data);
            };
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "WaitingScene") return;
            var go = GameObject.Find("NpcSpawnOffset");
            npcSpawnOffset = go?.transform;
        }

        public void LoadNpcList(List<UniverseNpc> npcList)
        {
            if (!photonView.IsMine) return;
            var go = GameObject.Find("NpcSpawnOffset");
            npcSpawnOffset = go?.transform;
            if (currentNpcList.Count > 0)
            {
                currentNpcList.ForEach(Destroy);
                currentNpcList.Clear();
            }


            foreach (var info in npcList)
            {
                var npc = spawner.PunSpawn(info);
                currentNpcList.Add(npc);
            }
        }


        /// <summary>
        /// PUN으로 요청되야함
        /// </summary>
        /// <param name="npcId"></param>
        public void InteractNpc(int npcId)
        {
            var npcInfo = currentNpcList.First(t => t.NpcId == npcId);
            currentInteractInGameNpc = npcInfo;
            turnSystem.InitTurn();
            if (PhotonNetwork.IsMasterClient)
            {
                var currentBackgroundName = ViewModel.UniverseData.backgroundPartDataList
                    .First(t => t.ID == ViewModel.CurrentBackgroundId).Name;
                StartCoroutine(
                    PlayProgressApi.StartNpcTalk(
                        PlayUniverseManager.Instance.roomNumber,
                        currentBackgroundName,
                        currentInteractInGameNpc.NpcName,
                        (_) => { }
                    )
                );
            }

            StartCoroutine(TurnBasedConversation());
        }


        public void OnSubmitChatClicked()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (ViewModel.NpcChatSelectedIndex == -1)
                {
                    // todo : show error message "선택지를 선택해주세요"
                    return;
                }

                if (ViewModel.NpcChatSelectedIndex == 0)
                {
                    selectorChat.Select(
                        ViewModel.CurrentPlayer.Nickname,
                        NpcChatUIManager.ChatInputField.text
                    );
                }
                else
                {
                    selectorChat.Select(ViewModel.NpcChatSelectedIndex);
                }

                NpcChatUIManager.SetChattable(false);
                ViewModel.NpcChatSelectedIndex = -1;
            }
            else //일반 사용자가 텍스트 입력을 했을때
            {
                selectorChat.AddOption(NpcChatUIManager.ChatInputField.text);
                NpcChatUIManager.SetChattable(false);
            }
        }

        public void FinishPlayerTurn()
        {
            //clear data
            RPC_FinishTurn();
        }

        [PunRPC]
        private void RPC_FinishTurn()
        {
            print($"this is Called by {PhotonNetwork.LocalPlayer.NickName}");
            ViewModel.NpcChatSelectedIndex = -1;

            selectorChat.ClearOptions();
            NpcChatUIManager.ClearChatOptions();
            NpcChatUIManager.HideChatOptions();
            NpcChatUIManager.SetChattable(false);
        }

        private IEnumerator ConversationWithNpc_Master()
        {
            print("기다림 시작");
            yield return new WaitUntil(() => selectorChat.OptionCount >= ViewModel.UniversePlayers.Count - 1);
            print("기다림 끝");
            NpcChatUIManager.SetChattable(true);
            selectorChat.ShowSelectors();
        }

        private IEnumerator ConversationWithNpc_Other()
        {
            NpcChatUIManager.SetChattable(true);
            yield return new WaitUntil(() => selectorChat.OptionCount >= ViewModel.UniversePlayers.Count - 1);
        }

        public void NextTurn()
        {
            selectorChat.ClearOptions();
            StartCoroutine(TurnBasedConversation());
        }

        IEnumerator TurnBasedConversation()
        {
            StartTurn();
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"마스터 대화 시작");
                Debug.Log($"Clear Options");
                yield return ConversationWithNpc_Master();
            }
            else
            {
                yield return ConversationWithNpc_Other();
            }
        }

        private void StartTurn()
        {
            var turn = turnSystem.GetNextTurn();
            PlayUniverseManager.Instance.NpcChatUIManager.SetTurnText(turn, "");
            PlayUniverseManager.Instance.NpcChatUIManager.SetChattable(false);
        }


        public void ShowNpcHpBar()
        {
            Debug.Log($"currentInterACTnPC : {currentInteractInGameNpc.name}");
            if (currentInteractInGameNpc.root != null)
                currentInteractInGameNpc.root.SetActive(true);
        }

        public void HideNpcHpBar()
        {
            if (currentInteractInGameNpc.root != null)
                currentInteractInGameNpc.root.SetActive(false);
        }

        public void FinishConversation()
        {
            StopAllCoroutines();
            NpcChatUIManager.Hide();
            NpcChatUIManager.SetChattable(false);
        }


        public void AddNpc(InGameNpc inGameNpc)
        {
            Debug.Log($"add npc : {inGameNpc}");
            if(npcSpawnOffset == null) npcSpawnOffset = GameObject.Find("NpcSpawnOffset").transform;
            inGameNpc.transform.SetParent(npcSpawnOffset);
            Debug.Log($"npcSpawnOffset : {npcSpawnOffset}");
            currentNpcList.Add(inGameNpc);
        }
        
        [PunRPC]
        private void RPC_ApplyGameMasterChatBubble(string content)
        {
            NpcChatUIManager.AddGameMasterChatBubble(content);
        }
    }
}