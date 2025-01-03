﻿using System;
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
        public bool isBlocked = false;
        public Action OnStartInteractNpc;
        public Action OnFinishInteractNpc;
        private static readonly int TriggerTalking = Animator.StringToHash("tirggerTalking");
        private static readonly int TriggerIdle = Animator.StringToHash("IsIdle");

        private void Start()
        {
            NpcChatUIManager.ChatInputField.onEndEdit.AddListener(
                (text) =>
                {
                    if (string.IsNullOrEmpty(text)) return;
                    // 엔터 키를 눌러서 끝냈는지 확인
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        OnSubmitChatClicked();
                    }
                }
            );
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameMasterServerEventManager.Instance.OnEventReceived += (data) =>
            {
                foreach (var d in data)
                {
                    if(d == null) continue;
                    photonView.RPC(nameof(RPC_ApplyGameMasterChatBubble), RpcTarget.All, d);
                }
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
            if (isBlocked) return;
            OnStartInteractNpc?.Invoke();
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
                        (npcFirstChat) =>
                        {
                            photonView.RPC(nameof(Pun_OnStartChat), RpcTarget.All);
                            photonView.RPC(nameof(Pun_AddChat), RpcTarget.All, npcInfo.NpcName, npcFirstChat.value);
                        }
                    )
                );
            }

            StartCoroutine(TurnBasedConversation());
        }


        [PunRPC]
        private void Pun_OnStartChat()
        {
            NpcChatUIManager.SetChattable(true);
            TalkingAnim(true);
        }
        
        [PunRPC]
        private void Pun_AddChat(string npcName, string content)
        {
            NpcChatUIManager.AddChatBubble(npcName, content, false);
        }

        public void OnSubmitChatClicked()
        {
            if(NpcChatUIManager.GetChattable() == false) return;
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
            
            NpcChatUIManager.ChatInputField.text = "";
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

        private Coroutine finishCoroutine;
        
        public void NextTurn()
        {
            TalkingAnim(true);
            if(finishCoroutine != null) StopCoroutine(finishCoroutine);
            finishCoroutine = StartCoroutine(FinishAnimAfter(3f));
            selectorChat.ClearOptions();
            StartCoroutine(TurnBasedConversation());
        }

        private IEnumerator FinishAnimAfter(float sec)
        {
            yield return new WaitForSeconds(sec);
            TalkingAnim(false);
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

        public void TalkingAnim(bool isTalking)
        {
            if (isTalking)
            {
                var fa = currentInteractInGameNpc.GetComponentInChildren<Animator>();
                fa.SetTrigger(TriggerTalking);
            }
            else
            {
                currentInteractInGameNpc.GetComponentInChildren<Animator>().SetTrigger(TriggerIdle);
            }
        }

        private void StartTurn()
        {
            var turn = turnSystem.GetNextTurn();
            PlayUniverseManager.Instance.NpcChatUIManager.SetTurnText(turn, "");
            PlayUniverseManager.Instance.NpcChatUIManager.SetChattable(false);
        }


        public void FinishConversation()
        {
            StopAllCoroutines();
            NpcChatUIManager.Hide();
            NpcChatUIManager.SetChattable(false);
            TalkingAnim(false);
            OnFinishInteractNpc?.Invoke();
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