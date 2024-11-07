using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using ViewModels;

namespace UniversePlay
{
    public class PlayNpcManager : MonoBehaviourPun
    {
        // private List<NpcInfo> currentBackgroundNPCList = new();

        [SerializeField]
        private List<NpcInPlay> currentNpcList = new();

        public CinemachineVirtualCamera CurrentNpcVcam => currentInteractNpc.ncVcam;
        private NpcInPlay currentInteractNpc;

        private TurnSystem turnSystem = new();

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;
        private NpcChatUIManager NpcChatUIManager => PlayUniverseManager.Instance.NpcChatUIManager;
        private static InGamePlayerManager PlayerManager => PlayUniverseManager.Instance.InGamePlayerManager;
        private int currentPlayerId = -1;

        public ISelectorChat selectorChat;

        private readonly NpcSpawner spawner = new NpcSpawner();


        public void LoadNpcList(List<NpcInfo> npcList)
        {
            if (!photonView.IsMine) return;
            
            if (currentNpcList.Count > 0)
            {
                currentNpcList.ForEach(Destroy);
                currentNpcList.Clear();
            }

            
            foreach (var info in npcList)
            {
                spawner.PunSpawn(info);
            }
        }


        public void InteractNpc(int npcId)
        {
            var npcInfo =currentNpcList.First(t => t.NpcId == npcId);
            currentInteractNpc = npcInfo;
            turnSystem.InitTurn();
            StartCoroutine(TurnBasedConversation());
        }
        
        [PunRPC]
        private void InitPlay()
        {
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

                if (ViewModel.NpcChatSelectedIndex == selectorChat.OptionCount)
                {
                    selectorChat.Select("Master", NpcChatUIManager.ChatInputField.text);
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


        private IEnumerator ConversationWithNpc_Master()
        {
            yield return new WaitUntil(() => selectorChat.OptionCount >= PlayerManager.PlayerCount);
            NpcChatUIManager.SetChattable(true);
            selectorChat.ShowSelectors();
        }

        private IEnumerator ConversationWithNpc_Other()
        {
            yield return new WaitUntil(() => selectorChat.OptionCount > PlayerManager.PlayerCount);
        }

        public void NextTurn()
        {
            StartCoroutine(TurnBasedConversation());
        }

        IEnumerator TurnBasedConversation()
        {
            StartTurn();
            if (PhotonNetwork.IsMasterClient)
            {
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
            Debug.Log($"currentInterACTnPC : {currentInteractNpc.name}");
            if (currentInteractNpc.root != null)
                currentInteractNpc.root.SetActive(true);
        }

        public void HideNpcHpBar()
        {
            if (currentInteractNpc.root != null)
                currentInteractNpc.root.SetActive(false);
        }

        public void FinishConversation()
        {
            StopAllCoroutines();
            NpcChatUIManager.Hide();
            NpcChatUIManager.SetChattable(false);
        }

        public void AddNpc(NpcInPlay npcInPlay)
        {
            currentNpcList.Add(npcInPlay);
        }
    }
}