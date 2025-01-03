﻿using System.ComponentModel;
using Data.Models.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniversePlay;
using ViewModels;

namespace UI.UniversePlay
{
    public class InGameHUDController : MonoBehaviour
    {
        public bool testWithKeyboard = false;

        [Header("오른쪽 HUD")]
        [SerializeField]
        private Button inAndOutButton;

        [SerializeField]
        private RectTransform chatPanel; // UI Panel의 RectTransform

        [SerializeField]
        private float rightHUDAnimateTime = 0.5f; // 애니메이션 지속 시간

        public bool isChatPanelVisible = true; // 현재 패널 상태 (보이는지 여부)
        private float chatPanelWidth;
        
        [Header("아랫쪽 HUD")]
        [SerializeField]
        private RectTransform dicePanel;

        [SerializeField]
        private TMP_Text guideText;
        
        [SerializeField]
        private float bottomHUDAnimateTime = 0.5f; // 애니메이션 지속 시간

        public bool isDicePanelVisible = false;
        private float dicePanelHeight;
        
        private const string DiceGuideText = "<color=#95FF00>스페이스바</b></color>를 눌러\n판정을 위한 <color=#95FF00>주사위</color>를 굴려주세요!";
        private const string BattleDiceGuideText = "<color=#95FF00>전투 판정</b></color>을 위한\n <color=#95FF00>주사위</color>가 굴러가고 있습니다!";
        
        UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

        private void Start()
        {
            dicePanelHeight = dicePanel.rect.height;
            chatPanelWidth = chatPanel.rect.width;
            inAndOutButton.onClick.AddListener(OnInAndOutButtonClick);
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            isChatPanelVisible = ViewModel.HUDState.HasFlag(EHUDState.Chat);
            if (isChatPanelVisible) AnimateIn();
            else AnimateOut();
            isDicePanelVisible = ViewModel.HUDState.HasFlag(EHUDState.Dice);
            if (isDicePanelVisible) AnimateDiceIn();
            else AnimateDiceOut();
        }

        private void Update()
        {
            if (testWithKeyboard && Input.GetKeyDown(KeyCode.A))
            {
                if (isDicePanelVisible)
                {
                    ViewModel.RemoveHUDState(EHUDState.Dice);
                }
                else
                {
                    ViewModel.AddHUDState(EHUDState.Dice);
                }
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.Log($"ViewModel Property Changed Called");
            if (e.PropertyName == nameof(ViewModel.HUDState))
            {
                if (ViewModel.HUDState.HasFlag(EHUDState.Chat) && !isChatPanelVisible)
                {
                    guideText.text = DiceGuideText;
                    isChatPanelVisible = true;
                    AnimateIn();
                }
                else if (!ViewModel.HUDState.HasFlag(EHUDState.Chat) && isChatPanelVisible)
                {
                    guideText.text = "";
                    isChatPanelVisible = false;
                    AnimateOut();
                }

                if (ViewModel.HUDState.HasFlag(EHUDState.Dice) && !isDicePanelVisible)
                {
                    isDicePanelVisible = true;
                    AnimateDiceIn();
                }
                else if (!ViewModel.HUDState.HasFlag(EHUDState.Dice) && isDicePanelVisible)
                {
                    isDicePanelVisible = false;

                    AnimateDiceOut();
                }
                else if (ViewModel.HUDState.HasFlag(EHUDState.Battle) && !isDicePanelVisible)
                {
                    guideText.text = BattleDiceGuideText;
                    isDicePanelVisible = true;
                    AnimateDiceIn();
                }
                else if (!ViewModel.HUDState.HasFlag(EHUDState.Battle) && isDicePanelVisible)
                {
                    guideText.text = "";
                    isDicePanelVisible = false;
                    AnimateDiceOut();
                }
            }
        }

        private void OnInAndOutButtonClick()
        {
            if (isChatPanelVisible)
            {
                ViewModel.RemoveHUDState(EHUDState.Chat);
            }
            else
            {
                ViewModel.AddHUDState(EHUDState.Chat);
            }
        }

        /// <summary>
        /// 패널을 오른쪽으로 사라지게 하는 애니메이션
        /// </summary>
        private void AnimateOut()
        {
            iTween.MoveTo(
                chatPanel.gameObject,
                iTween.Hash(
                    "x",
                    483,
                    "islocal",
                    true,
                    "time",
                    rightHUDAnimateTime,
                    "easetype",
                    iTween.EaseType.easeOutCubic
                )
            );
        }

        /// <summary>
        /// 패널을 화면 안으로 나타나게 하는 애니메이션
        /// </summary>
        private void AnimateIn()
        {
            iTween.MoveTo(
                chatPanel.gameObject,
                iTween.Hash(
                    "x",
                    0,
                    "islocal",
                    true,
                    "time",
                    rightHUDAnimateTime,
                    "easetype",
                    iTween.EaseType.easeOutCubic
                )
            );
        }

        private void AnimateDiceIn()
        {
            iTween.MoveTo(
                dicePanel.gameObject,
                iTween.Hash(
                    "y",
                    0,
                    "islocal",
                    true,
                    "time",
                    rightHUDAnimateTime,
                    "easetype",
                    iTween.EaseType.easeOutCubic
                )
            );
        }

        private void AnimateDiceOut()
        {
            iTween.MoveTo(
                dicePanel.gameObject,
                iTween.Hash(
                    "y",
                    -220,
                    "islocal",
                    true,
                    "time",
                    rightHUDAnimateTime,
                    "easetype",
                    iTween.EaseType.easeOutCubic
                )
            );
        }


        private void OnDestroy()
        {
            inAndOutButton.onClick.RemoveListener(OnInAndOutButtonClick);
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}