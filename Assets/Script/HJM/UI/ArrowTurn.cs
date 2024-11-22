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
        // 이미지의 머티리얼을 가져옵니다.
        arrowMaterial = arrowImage.material;

        initialPosition = arrowImage.transform.position;
        StartCoroutine(ArrowBlinkAndMove());
    }

    private IEnumerator ArrowBlinkAndMove()
    {
        float blinkInterval = 1f / blinkSpeed;
        float time = 0f;

        while (true)
        {
            // 알파값을 계산합니다 (0에서 0.5까지 깜빡이게)
            float alpha = Mathf.PingPong(time * blinkSpeed, 0.5f);
            Color currentColor = arrowImage.color;

            // Image의 알파값 수정
            currentColor.a = alpha;
            arrowImage.color = currentColor;

            // 머티리얼의 알파값을 수정
            if (arrowMaterial.HasProperty("_Color"))
            {
                // 머티리얼의 알파값을 변경합니다.
                Color materialColor = arrowMaterial.color;
                materialColor.a = alpha;
                arrowMaterial.color = materialColor;
            }

            // 이미지의 위치를 위아래로 움직이게 합니다.
            float moveOffset = Mathf.Sin(time * Mathf.PI * 2f * 0.5f) * moveAmount;
            arrowImage.transform.position = initialPosition + new Vector3(0f, moveOffset, 0f);

            time += Time.deltaTime;

            yield return null;
        }
    }
}
