﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Data.Models.Universe;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Dice;
using Data.Remote.Api;
using Photon.Pun;
using UnityEngine;
using Utils;
using ViewModels;
using Random = UnityEngine.Random;

namespace UniversePlay
{
    /// <summary>
    /// Pun을 활용한 Npc 채팅 선택지 시스템
    /// UI는 따로 UIController를 사용할 예정
    /// </summary>
    public class PunSelectorChat : MonoBehaviourPun, ISelectorChat
    {
        //todo : user info
        private readonly List<KeyValuePair<string, string>> options = new();

        public int OptionCount => options.Count;

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

        private NpcChatUIManager ChatUIManager => PlayUniverseManager.Instance.NpcChatUIManager;
        private InGameNpcManager NpcManager => PlayUniverseManager.Instance.NpcManager;

        private bool isDiceRollReady = false;
        private NpcReaction currentNpcReaction;
        private void Update()
        {
            if (isDiceRollReady)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RollDice(currentNpcReaction, NpcManager.currentInteractInGameNpc.NpcName);
                    isDiceRollReady = false;
                }
            }
        }

        public void AddOption(string option)
        {
            photonView.RPC(nameof(RPC_AddOption), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName, option);
        }

        [PunRPC]
        private void RPC_AddOption(string nickname, string option)
        {
            options.Add(new KeyValuePair<string, string>(nickname, option));
        }


        public void ShowSelectors()
        {
            options.Insert(0, new KeyValuePair<string, string>("", "직접 입력"));
            ChatUIManager.ShowChatOptions(options);
        }

        public void Select(int index)
        {
            var sender = options.ElementAt(index).Key;
            var option = options.ElementAt(index).Value;
            Select(sender, option);
        }

        public void Select(string sender, string option)
        {
            var roomNumber = PlayUniverseManager.Instance.roomNumber;
            Debug.Log($"NPC한테 보냅니다");
            NpcManager.FinishPlayerTurn();
            photonView.RPC(nameof(RPC_ApplyChatBubble), RpcTarget.All, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    roomNumber,
                    option,
                    (res) =>
                    {
                        Debug.Log($"받았습니다 {res.value}");
                        if (res.IsSuccess)
                        {
                            OnNpcReaction(sender, res.value);
                        }
                    }
                )
            );
        }

        public void ClearOptions()
        {
            options.Clear();
        }

        [PunRPC]
        private void RPC_ApplyChatBubble(string sender, string chatContent)
        {
            var isNpc = sender == NpcManager.currentInteractInGameNpc.NpcName;
            ChatUIManager.AddChatBubble(sender, chatContent, !isNpc);
        }
        

        [PunRPC]
        private void RPC_ToNextTurn()
        {
            NpcManager.NextTurn();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int, int) GetTwoRandom()
        {
            return (Random.Range(1, 7), Random.Range(1, 7));
        }

        /// <summary>
        /// 이 함수는 방장에 의해서만 호출된다. (즉 모든 플레이어중에 한명만 실행한다)
        /// </summary>
        /// <param name="sender">채팅을 친 플레이어</param>
        /// <param name="reaction">Npc의 반응 응답 값</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void OnNpcReaction(string sender, NpcReaction reaction)
        {
            var npcName = NpcManager.currentInteractInGameNpc.NpcName;
            photonView.RPC(nameof(RPC_ApplyChatBubble), RpcTarget.All, npcName, reaction.DialogMessage);
            switch (reaction.ReactionType)
            {
                case EReactionType.None:
                    Debug.LogError($"{reaction.ReactionType} is not implemented");
                    break;
                case EReactionType.Progress:
                    photonView.RPC(nameof(RPC_ToNextTurn), RpcTarget.All);
                    Debug.Log($"Progress");
                    break;
                case EReactionType.Battle:
                    PlayUniverseManager.Instance.isBattle = true;
                    PlayUniverseManager.Instance.FinishConversation(endAiTalk:false);
                    BattleManager.Instance.StartBattle();
                    break;
                case EReactionType.Dice:
                    ViewModel.AddHUDState(EHUDState.Dice);
                    DiceRollManager.Get().DiceStandby();
                    currentNpcReaction = reaction;
                    isDiceRollReady = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RollDice(NpcReaction reaction, string npcName)
        {
            int stat;
            if (reaction.BonusMessage == "strength")
            {
                stat = ViewModel.CurrentPlayer.Stats.GetStat(EStatType.Str);
            }
            else if(reaction.BonusMessage == "dex")
            {
                stat = ViewModel.CurrentPlayer.Stats.GetStat(EStatType.Dex);
            }
            else
            {
                stat = 0;
            }
            
            Debug.Log($"Roll Dice with {stat}");
            

            DiceRollManager.Get().onDiceRollFinished = () =>
            {
                this.DoAfterSeconds(3,
                    () =>
                    {
                        ViewModel.RemoveHUDState(EHUDState.Dice);
                    });
            };
            DiceRollManager.Get().SearchDiceRoll(stat);
            var d1 = DiceRollManager.Get().diceResults[0];
            var d2 = DiceRollManager.Get().diceResults[1];
            StartCoroutine(
                PlayProgressApi.CheckDice(
                    PlayUniverseManager.Instance.roomNumber,
                    reaction.BonusMessage,
                    new DiceResult(
                        UserCodeMgr.Instance.UserCode,
                        firstDiceNumber: d1,
                        secondDiceNumber: d2
                    ),
                    (res) =>
                    {
                        Debug.Log($"Dice Result : {res.IsSuccess}");
                        if (res.IsSuccess)
                        {
                            OnNpcReaction(npcName, res.value);
                        }
                        else
                        {
                            Debug.LogError($"Something Wrong When On Dice Result: {res.error}");
                        }
                    }
                )
            );
        }
    }
}