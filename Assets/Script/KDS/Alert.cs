using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    //싱글톤
    public static Alert instance;

    public static Alert Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Alert");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<Alert>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
    }

    public GameObject alertCanvas;

    public TMP_Text alertText;

    //코루틴 변수
    private IEnumerator coroutine;

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

    public void Set(string text, float time)
    {
        TextSetUp(text);

        coroutine = alertStart(time);

        StartCoroutine(coroutine);
    }

    public void Set(string text)
    {
        TextSetUp(text);

        coroutine = alertStart(1.5f);

        StartCoroutine(coroutine);
    }

    void TextSetUp(string text)
    {
        //초기화
        alertText.text = " ";

        alertCanvas.SetActive(false);

        //기존 코루틴 스탑
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        alertText.text = text;
    }

    IEnumerator alertStart(float time)
    {
        alertCanvas.SetActive(true);

        yield return new WaitForSeconds(time);

        alertCanvas.SetActive(false);
    }
}
