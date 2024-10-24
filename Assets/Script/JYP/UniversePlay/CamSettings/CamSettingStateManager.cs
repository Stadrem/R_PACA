﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CamSettingStateManager : MonoBehaviour
{
    public enum ECamSettingStates
    {
        None = -1,
        QuarterView,
        TalkView,
        BattleView,
    }

    private Dictionary<ECamSettingStates, ICamSettingState> camSettingStates = new();
    
    [CanBeNull]
    private ICamSettingState currentCamSetting = null;

    private ECamSettingStates currentState = ECamSettingStates.None;

    private void Awake()
    {
        camSettingStates.Add(ECamSettingStates.TalkView, new TalkViewCamSetting());
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