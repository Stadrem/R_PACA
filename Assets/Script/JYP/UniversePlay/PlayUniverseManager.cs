﻿using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UniversePlay;
using ViewModels;

/// <summary>
/// 플레이 화면에서의 전체적인 매니저
/// 모든 매니저들을 관리하고, 유저의 상호작용을 처리하는 클래스
/// </summary>
public class PlayUniverseManager : MonoBehaviour
{
    private static PlayUniverseManager instance;

    [SerializeField] private PlayBackgroundManager playBackgroundManager;

    public PlayBackgroundManager BackgroundManager => playBackgroundManager;

    [SerializeField] private PlayNpcManager playNpcManager;

    public PlayNpcManager NpcManager => playNpcManager;

    [SerializeField] private NpcChatUIManager npcChatUIManager;

    public NpcChatUIManager NpcChatUIManager => npcChatUIManager;

    [SerializeField] private CamSettingStateManager camSettingManager;

    public CamSettingStateManager CamSettingManager => camSettingManager;

    [SerializeField] private InGamePlayerManager inGamePlayerManager;

    public InGamePlayerManager InGamePlayerManager => inGamePlayerManager;

    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

    public static PlayUniverseManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = PhotonNetwork.Instantiate(
                    "UniversePlay/UniversePlayManager",
                    Vector3.zero,
                    Quaternion.identity
                );
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        ViewModel.LoadUniverseData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ShowBattleUI();
        }

        UserInteraction();
    }


    private void UserInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue))
            {
                if (hit.collider.CompareTag("InPlayNPC"))
                {
                    playNpcManager.InteractNpc(hit.collider.GetComponent<NpcInPlay>());
                    CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.TalkView);
                }
                else if (hit.collider.CompareTag("Portal"))
                {
                    hit.collider.GetComponent<PortalInPlay>()
                        ?.InteractByUser();
                }
            }
        }
    }


    public void FinishConversation()
    {
        CamSettingManager.TransitState(CamSettingStateManager.ECamSettingStates.QuarterView);
    }

    /// <summary>
    /// 전투화면에서의 UI를 보여주는 함수, 등장인물들의 HP UI를 보여준다.
    /// NPC의 경우, 대화를 시작했다는 가정하에 대화하고 있던 Npc의 HP UI를 보여준다.
    /// </summary>
    public void ShowBattleUI()
    {
        NpcManager.ShowNpcHpBar();
        InGamePlayerManager.ShowPlayersHpBar();
    }

    /// <summary>
    /// 전투화면에서의 UI를 숨기는 함수, 등장인물들의 HP UI를 숨긴다.
    /// </summary>
    public void HideBattleUI()
    {
        NpcManager.HideNpcHpBar();
        InGamePlayerManager.HidePlayersHpBar();
    }

    public void StartPlay()
    {
        if (PhotonNetwork.IsMasterClient)
            playBackgroundManager.Init();
    }
}