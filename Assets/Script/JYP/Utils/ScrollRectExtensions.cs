using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class ScrollRectExtensions
    {
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            Canvas.ForceUpdateCanvases(); // 레이아웃 강제 업데이트
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}