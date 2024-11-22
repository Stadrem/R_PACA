using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowTurn : MonoBehaviour
{
    public Image arrowImage;
    public float blinkSpeed = 1.0f;
    public float moveAmount = 1.0f;

    private Vector3 initialPosition;
    private Material arrowMaterial;

    private void Start()
    {
        arrowMaterial = new Material(arrowImage.material);
        arrowImage.material = arrowMaterial;

       
        initialPosition = arrowImage.transform.position;

        
        StartCoroutine(ArrowBlinkAndMove());
    }

    private IEnumerator ArrowBlinkAndMove()
    {
        float time = 0f;

        while (true)
        {
            
            float alpha = Mathf.PingPong(time * blinkSpeed, 0.5f);
            Color currentColor = arrowImage.color;
            currentColor.a = alpha;
            arrowImage.color = currentColor;
            
            if (arrowMaterial.HasProperty("_Color"))
            {
                Color materialColor = arrowMaterial.color;
                materialColor.a = alpha;
                arrowMaterial.color = materialColor;
            }

            float moveOffset = Mathf.Sin(time * Mathf.PI * 2f * 0.5f) * moveAmount;
            arrowImage.transform.position = initialPosition + new Vector3(0f, moveOffset, 0f);

            time += Time.deltaTime;
            yield return null;
        }
    }
}
