using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class CamSettingStateManager : MonoBehaviourPun
{
    public enum ECamSettingStates
    {
        None = -1,
        QuarterView,
        TalkView,
        BattleView,
    }

    private Dictionary<ECamSettingStates, ICamSettingState> camSettingStates = new();

    [CanBeNull] private ICamSettingState currentCamSetting = null;

    private ECamSettingStates currentState = ECamSettingStates.None;

    private void Awake()
    {
        camSettingStates.Add(ECamSettingStates.QuarterView, new QuarterViewState());
        camSettingStates.Add(ECamSettingStates.TalkView, new TalkViewCamSetting());
        currentState = ECamSettingStates.QuarterView;
        currentCamSetting = camSettingStates[currentState];
    }

    private void Update()
    {
        currentCamSetting?.OnUpdate();
    }

    public void TransitState(ECamSettingStates state)
    {
        if (currentState == state) return;

        currentCamSetting?.OnExit();
        currentState = state;
        currentCamSetting = camSettingStates[state];
        currentCamSetting?.OnEnter();
    }
}