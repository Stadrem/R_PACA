using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.Models.Universe;
using Data.Models.Universe.Characters;
using Data.Remote;
using Data.Remote.Api;
using Data.Remote.Dtos.Response;
using Script.JYP.UniversePlay.Chat;
using UnityEngine;

namespace UniversePlay
{
    public sealed partial class UniversePlayViewModel : INotifyPropertyChanged
    {
        #region Properties

        private List<ChatLog> chatLogs = new();
        private int npcChatSelectedIndex = -1;
        private UniverseData universeData;
        private int currentBackgroundId = -1;
        private EHUDState hudState = 0;
        
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion


        public EHUDState HUDState
        {
            get => hudState;
            set => SetField(ref hudState, value);
        }

        private string introMessage;

        public void AddHUDState(EHUDState state)
        {
            hudState |= state;
            OnPropertyChanged(nameof(HUDState));
        }

        public void RemoveHUDState(EHUDState state)
        {
            hudState &= ~state;
            OnPropertyChanged(nameof(HUDState));
        }


        public int NpcChatSelectedIndex
        {
            get => npcChatSelectedIndex;
            set => SetField(ref npcChatSelectedIndex, value);
        }

        public UniverseData UniverseData
        {
            get => universeData;
            set => SetField(ref universeData, value);
        }

        public int CurrentBackgroundId
        {
            get => currentBackgroundId;
            set
            {
                if (value >= 0)
                {
                    currentMapNpcList = UniverseData.backgroundPartDataList.Find((info) => info.ID == value).NpcList;
                }

                SetField(ref currentBackgroundId, value);
            }
        }

        public IEnumerator TalkNpc(int roomNumber, string message, Action<ApiResult<NpcReaction>> callback)
        {
            yield return PlayProgressApi.SendChat(
                roomNumber,
                message,
                (res) => { callback(res.Map((t) => t.ToReaction())); }
            );
        }

        /// <summary>
        /// 플레이할 세계관(시나리오)의 데이터를 불러온다
        /// </summary>
        /// <param name="universeId">불러올 데이터의 키값, scenarioId</param>
        /// <returns></returns>
        public IEnumerator LoadUniverseData(int universeId) //todo 아직 더미 데이터
        {
            yield return ScenarioApi.GetScenario(
                universeId,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        if (result.value.backgroundPartDataList.Count == 0) // for test 
                        {
                            Debug.LogError($"배경없는 세계관입니다. {universeId}");
                            return;
                        }

                        UniverseData = result.value;
                    }
                }
            );
        }

        public IEnumerator StartRoom(int roomNumber, string title, List<int> playerIds, Action<ApiResult> callback)
        {
            Debug.Log($"Start Room - {roomNumber}");
            yield return PlayRoomApi.StartRoom(
                roomNumber,
                title,
                UniverseData.id,
                playerIds,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        callback(ApiResult.Success());
                        Debug.Log("Room Started");
                        introMessage = result.value;
                        Debug.Log($"intro: {introMessage}");
                    }
                    else
                    {
                        callback(ApiResult.Fail(result.error));
                    }
                }
            );
        }

        public void GoToNextBackground()
        {
            Debug.LogWarning($"current id - {CurrentBackgroundId}");
            var nextIdx = UniverseData.backgroundPartDataList.FindIndex((t) => t.ID == CurrentBackgroundId) + 1;
            if (nextIdx >= UniverseData.backgroundPartDataList.Count)
            {
                Debug.Log("마지막 배경입니다.");
                return;
            }
            
            PlayUniverseManager.Instance.BackgroundManager.SetCurrentBackgroundId(UniverseData.backgroundPartDataList[nextIdx].ID);
            
            Debug.LogWarning($"next id - {CurrentBackgroundId}");
            
        }
    }
}