using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        chatLogs.Add(new ChatLog { talker = sender, message = message });
        chatLogs.Add(new ChatLog { talker = sender, message = "Hello!" });
        callback(
            ApiResult<NpcChatResponseDto>.Success(
                new NpcChatResponseDto()
                {
                    sender = sender,
                    message = message,
                    isBattle = false,
                    isQuestAchieved = false
                }
            )
        );
    }

    public void LoadUniverseData() //todo 아직 더미 데이터
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
                Name = "마을사람 1",
                Position = new Vector3(58.8600006f, 9.57999992f, 65.8899994f),
                Type = NpcInfo.ENPCType.Human,
            },

            new NpcInfo()
            {
                Name = "고블린 1",
                Position = new Vector3(62.4500008f, 9.45199966f, 66.5199966f),
                Type = NpcInfo.ENPCType.Goblin,
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

        var universe = new UniverseData()
        {
            id = 0,
            name = "Universe 0",
            backgroundPartDataList = backgroundList
        };
        
        universeData = universe;
    }
}