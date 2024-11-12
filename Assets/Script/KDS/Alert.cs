using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    //알림창 기능. 알아서 프리팹 가져옴.
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

    //감추고 표시할 캔버스
    public GameObject alertCanvas;

    //변경할 텍스트 내용
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

    //텍스트와 시간 포함 된 오버로딩 1
    public void Set(string text, float time)
    {
        TextSetUp(text);

        coroutine = alertStart(time);

        StartCoroutine(coroutine);
    }

    //텍스트만 넣는 오버로딩 2
    public void Set(string text)
    {
        TextSetUp(text);

        coroutine = alertStart(1.5f);

        StartCoroutine(coroutine);
    }

    //초기화 겸 텍스트 넣기
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

    //코루틴
    IEnumerator alertStart(float time)
    {
        SoundManager.Get().PlaySFX(2);

        alertCanvas.SetActive(true);

        yield return new WaitForSeconds(time);

        alertCanvas.SetActive(false);
    }
}
