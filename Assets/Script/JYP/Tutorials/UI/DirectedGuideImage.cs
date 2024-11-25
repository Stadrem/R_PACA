using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DirectedGuideImage : MonoBehaviour
{
    [SerializeField]
    private float blinkTime = 0.5f;

    [SerializeField]
    private Image image;

    private float currentTime = 0;
    private int blinkState = 1;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= blinkTime)
        {
            blinkState = blinkState == 1 ? 0 : 1;
            image.color = blinkState == 1 ? Color.white : Color.red;
            currentTime =0f;
        }
    }
}