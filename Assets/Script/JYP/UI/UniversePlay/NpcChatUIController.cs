using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UniversePlay
{
    public class NpcChatUIController : MonoBehaviour
    {
        [SerializeField]
        private Button inAndOutButton;

        [SerializeField]
        private RectTransform chatPanel; // UI Panel의 RectTransform

        [SerializeField]
        private float animateTime = 0.5f; // 애니메이션 지속 시간

        private bool isPanelVisible = true; // 현재 패널 상태 (보이는지 여부)

        private void Start()
        {
            inAndOutButton.onClick.AddListener(OnInAndOutButtonClick);
        }

        public void OnInAndOutButtonClick()
        {
            if (isPanelVisible)
            {
                AnimateOut();
                inAndOutButton.GetComponentInChildren<TMP_Text>()
                    .text = ">";
            }
            else
            {
                AnimateIn();
                inAndOutButton.GetComponentInChildren<TMP_Text>()
                    .text = "<";
            }

            isPanelVisible = !isPanelVisible;
        }
        /// <summary>
        /// 패널을 오른쪽으로 사라지게 하는 애니메이션
        /// </summary>
        private void AnimateOut()
        {
            iTween.MoveTo(chatPanel.gameObject, iTween.Hash(
                "x", 0,
                "islocal", true,
                "time", animateTime,
                "easetype", iTween.EaseType.easeOutCubic,
                "oncomplete", nameof(OnAnimateOutComplete),
                "oncompletetarget", gameObject
            ));
        }

        /// <summary>
        /// 패널을 화면 안으로 나타나게 하는 애니메이션
        /// </summary>
        private void AnimateIn()
        {
            iTween.MoveTo(chatPanel.gameObject, iTween.Hash(
                "x", -400,
                "islocal", true,
                "time", animateTime,
                "easetype", iTween.EaseType.easeOutCubic,
                "oncomplete", nameof(OnAnimateInComplete),
                "oncompletetarget", gameObject
            ));
        }

        private void OnAnimateOutComplete()
        {
            
        }
        
        private void OnAnimateInComplete()
        {
            
        }
    }
}