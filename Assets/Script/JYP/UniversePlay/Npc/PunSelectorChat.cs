using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Dice;
using Data.Remote.Api;
using Photon.Pun;
using UnityEngine;
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
        private PlayNpcManager NpcManager => PlayUniverseManager.Instance.NpcManager;

        private void Start()
        {
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
            var roomNumber = PlayUniverseManager.Instance.roomNumber;
            NpcManager.FinishPlayerTurn();

            photonView.RPC(nameof(RPC_ApplyChatBubble), RpcTarget.All, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    roomNumber,
                    option,
                    (res) =>
                    {
                        if (res.IsSuccess)
                        {
                            var resVal = res.value;
                            OnNpcReaction(sender, resVal);
                        }
                    }
                )
            );
        }

        public void Select(string sender, string option)
        {
            var roomNumber = PlayUniverseManager.Instance.roomNumber;
            Debug.Log($"NPC한테 보냅니다");
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
        private void RPC_ApplyChatBubble(string sender, string option)
        {
            ChatUIManager.AddChatBubble(sender, option, true);
        }

        [PunRPC]
        private void RPC_ToNextTurn()
        {
            NpcManager.NextTurn();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int, int) GetTowRandom()
        {
            return (Random.Range(1, 7), Random.Range(1, 7));
        }

        private void OnNpcReaction(string sender, NpcReaction reaction)
        {
            RPC_ApplyChatBubble(sender, reaction.DialogMessage);
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
                    break;
                case EReactionType.Dice:
                    var (d1, d2) = GetTowRandom();
                    Debug.Log($"Dice : {d1}, {d2}");
                    DiceRollManager.Get().DiceRoll(d1, d2, false);
                    StartCoroutine(
                        PlayProgressApi.CheckDice(
                            PlayUniverseManager.Instance.roomNumber,
                            new DiceResult(
                                firstDiceNumber: d1,
                                secondDiceNumber: d2
                            ),
                            (res) => { Debug.Log($"Dice Result : {res.IsSuccess}"); } // todo : Dice 잘됨?
                        )
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}