using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float step = 0.2f;

    public void MoveRight()
    {
        float newPosition = Mathf.Clamp(scrollRect.horizontalNormalizedPosition + step, 0, 1);
        scrollRect.horizontalNormalizedPosition = newPosition;
    }

    public void MoveLeft()
    {
        float newPosition = Mathf.Clamp(scrollRect.horizontalNormalizedPosition - step, 0, 1);
        scrollRect.horizontalNormalizedPosition = newPosition;
    }
}
