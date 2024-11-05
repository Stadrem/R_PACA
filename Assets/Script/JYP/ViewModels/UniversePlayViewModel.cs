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
}