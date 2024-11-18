using System.ComponentModel;
using Data.Models.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;

namespace UI.UniversePlay
{
    public class InGameHUDController : MonoBehaviour
    {
        [Header("오른쪽 HUD")]
        [SerializeField]
        private Button inAndOutButton;

        [SerializeField]
        private RectTransform chatPanel; // UI Panel의 RectTransform

        [SerializeField]
        private float animateTime = 0.5f; // 애니메이션 지속 시간

        private bool isChatPanelVisible = true; // 현재 패널 상태 (보이는지 여부)

        [Header("아랫쪽 HUD")]
        [SerializeField]
        private RectTransform dicePanel;

        private bool isDicePanelVisible = false;

        UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

        private void Start()
        {
            inAndOutButton.onClick.AddListener(OnInAndOutButtonClick);
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            isChatPanelVisible = ViewModel.HUDState.HasFlag(EHUDState.Chat);
            isDicePanelVisible = ViewModel.HUDState.HasFlag(EHUDState.Dice);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
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
            if (e.PropertyName == nameof(ViewModel.HUDState))
            {
                if (ViewModel.HUDState.HasFlag(EHUDState.Chat) && !isChatPanelVisible)
                {
                    isChatPanelVisible = true;
                    AnimateIn();
                }
                else if (!ViewModel.HUDState.HasFlag(EHUDState.Chat) && isChatPanelVisible)
                {
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
            }
        }

        private void OnInAndOutButtonClick()
        {
            if (isChatPanelVisible)
            {
                ViewModel.RemoveHUDState(EHUDState.Chat);
                inAndOutButton.GetComponentInChildren<TMP_Text>()
                    .text = "<";
            }
            else
            {
                ViewModel.AddHUDState(EHUDState.Chat);
                inAndOutButton.GetComponentInChildren<TMP_Text>()
                    .text = ">";
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
                    0,
                    "islocal",
                    true,
                    "time",
                    animateTime,
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
                    -400,
                    "islocal",
                    true,
                    "time",
                    animateTime,
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
                    animateTime,
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
                    -300,
                    "islocal",
                    true,
                    "time",
                    animateTime,
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