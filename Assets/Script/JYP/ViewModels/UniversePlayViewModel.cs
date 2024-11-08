using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.Remote;
using Script.JYP.UniversePlay.Chat;
using UnityEngine;

public sealed class UniversePlayViewModel : INotifyPropertyChanged
{
    #region Properties

    private List<NpcInfo> currentMapNpcList = new();
    private List<ChatLog> chatLogs = new();
    private int npcChatSelectedIndex = -1;
    private UniverseData universeData;
    private int currentBackgroundId = -1;

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
        set => SetField(ref currentBackgroundId, value);
    }

    public IEnumerator TalkNpc(string sender, string message, Action<ApiResult<NpcChatResponseDto>> callback)
    {
        // todo : send message thru API
        yield return new WaitForSeconds(0.5f);
        callback(
            ApiResult<NpcChatResponseDto>.Success(
                new NpcChatResponseDto()
                {
                    sender = "NPC",
                    message = "응답이오~",
                    isBattle = false,
                    isQuestAchieved = false
                }
            )
        );
    }

    /// <summary>
    /// 플레이할 세계관(시나리오)의 데이터를 불러온다
    /// </summary>
    /// <param name="universeId">불러올 데이터의 키값, scenarioId</param>
    /// <returns></returns>
    public IEnumerator LoadUniverseData(int universeId) //todo 아직 더미 데이터
    {
        var portalList1 = new List<PortalData>()
        {
            new PortalData()
            {
                position = new Vector3(58.4300003f, 9.30700016f, 51.8300018f),
                targetBackgroundId = 1,
            },
        };

        var portalList2 = new List<PortalData>()
        {
            new PortalData()
            {
                position = Vector3.zero,
                targetBackgroundId = 0,
            },
        };

        var npcList1 = new List<NpcInfo>()
        {
            new NpcInfo()
            {
                name = "마을사람 1",
                position = new Vector3(58.8600006f, 9.57999992f, 65.8899994f),
                npcShapeType = NpcInfo.ENpcType.Human,
            },

            new NpcInfo()
            {
                name = "고블린 1",
                position = new Vector3(62.4500008f, 9.45199966f, 66.5199966f),
                npcShapeType = NpcInfo.ENpcType.Goblin,
            }
        };
        var backgroundList = new List<BackgroundPartInfo>()
        {
            new BackgroundPartInfo()
            {
                ID = 0,
                Name = "Town 0",
                Type = EBackgroundPartType.Town,
                UniverseId = 0,
                PortalList = portalList1,
                NpcList = npcList1,
            },
            new BackgroundPartInfo()
            {
                ID = 1,
                Name = "Dungeon 0",
                Type = EBackgroundPartType.Dungeon,
                UniverseId = 0,
                PortalList = portalList2,
                NpcList = new List<NpcInfo>()
            },
        };

        yield return ScenarioApi.GetScenario(
            universeId,
            (result) =>
            {
                if (result.IsSuccess)
                {
                    if(result.value.backgroundPartDataList.Count == 0) // for test 
                    {
                        result.value.backgroundPartDataList = backgroundList;
                    }
                    UniverseData = result.value;
                }
            }
        );
    }
}