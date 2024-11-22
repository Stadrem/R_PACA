using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowTurn : MonoBehaviour
{
    public Image arrowImage;
    public float blinkSpeed = 1.0f;
    public float moveAmount = 1.0f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = arrowImage.transform.position;
        StartCoroutine(ArrowBlinkAndMove());
    }

    private IEnumerator ArrowBlinkAndMove()
    {
        float blinkInterval = 1f / blinkSpeed;
        float time = 0f;

        while (true)
        {
            
            float alpha = Mathf.PingPong(time * blinkSpeed, 1f);
            Color currentColor = arrowImage.color;
            currentColor.a = alpha;
            arrowImage.color = currentColor;

            
            float moveOffset = Mathf.Sin(time * Mathf.PI * 2f * 0.5f) * moveAmount;
            arrowImage.transform.position = initialPosition + new Vector3(0f, moveOffset, 0f);

            time += Time.deltaTime;

           
            yield return null;
        }
    }
}
