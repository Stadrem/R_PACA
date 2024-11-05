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


        private Dictionary<string, string> options = new();


        public int OptionCount => options.Count;

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

        private NpcChatUIManager ChatUIManager => PlayUniverseManager.Instance.NpcChatUIManager;
        private PlayNpcManager NpcManager => PlayUniverseManager.Instance.NpcManager;

        private void Start()
        {
        }


        public void AddOption(string option)
        {
            photonView.RPC("RPC_AddOption", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName, option);
        }

        [PunRPC]
        private void RPC_AddOption(string nickname, string option)
        {
            options.Add(nickname, option);
        }


        public void ShowSelectors()
        {
            ChatUIManager.ShowChatOptions(options);
        }

        public void Select(int index)
        {
            var sender = options.ElementAt(index).Key;
            var option = options.ElementAt(index).Value;

            photonView.RPC(nameof(RPC_Select), RpcTarget.MasterClient, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    sender,
                    option,
                    (res) =>
                    {
                        if (res.IsSuccess)
                            photonView.RPC(nameof(RPC_NpcResponse), RpcTarget.All, res.value);
                    }
                )
            );
        }

        public void Select(string sender, string option)
        {
            photonView.RPC(nameof(RPC_Select), RpcTarget.MasterClient, sender, option);
            StartCoroutine(
                ViewModel.TalkNpc(
                    sender,
                    option,
                    (res) =>
                    {
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

        [PunRPC]
        private void RPC_Select(string sender, string option)
        {
            ChatUIManager.AddChatBubble(sender, option);
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
                ChatUIManager.AddChatBubble(sender, message);
                NpcManager.NextTurn();
            }
        }
    }
}