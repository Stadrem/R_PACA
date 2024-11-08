using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using ViewModels;

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

            NpcManager.FinishPlayerTurn();

            photonView.RPC(nameof(RPC_ApplyChatBubble), RpcTarget.All, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    sender,
                    option,
                    (res) =>
                    {
                        if (res.IsSuccess)
                        {
                            var resVal = res.value;
                            photonView.RPC(
                                nameof(RPC_NpcResponse),
                                RpcTarget.All,
                                resVal.sender,
                                resVal.message,
                                resVal.isBattle,
                                resVal.isQuestAchieved
                            );
                        }
                    }
                )
            );
        }

        public void Select(string sender, string option)
        {
            Debug.Log($"NPC한테 보냅니다");
            photonView.RPC(nameof(RPC_ApplyChatBubble), RpcTarget.MasterClient, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    sender,
                    option,
                    (res) =>
                    {
                        Debug.Log($"받았습니다 {res.value}");
                        if (res.IsSuccess)
                        {
                            var val = res.value;
                            photonView.RPC(
                                nameof(RPC_NpcResponse),
                                RpcTarget.All,
                                val.sender,
                                val.message,
                                val.isBattle,
                                val.isQuestAchieved
                            );
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
        private void RPC_NpcResponse(string sender, string message, bool isBattle, bool isQuestAchieved)
        {
            if (isQuestAchieved)
            {
                // todo : quest achieved
                Debug.Log($"Quest Finished!");
            }
            else if (isBattle)
            {
                Debug.Log($"Battle Start");
            }
            else // just chat
            {
                ChatUIManager.AddChatBubble(sender, message, false);
                NpcManager.NextTurn();
            }
        }
    }
}