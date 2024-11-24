using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCinemachine : MonoBehaviourPunCallbacks
{
    public static BattleCinemachine Instance;

    public GameObject compositionUI;
    public GameObject battleUI;
    public GameObject cinemaUI;
    public GameObject MonsterDialogue;

    public Image backImage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        //battleUI = BattleManager.Instance.battleUI;
        StartAwakeCinema(); // 테스트용으로 스타트에서 시작
    }

    public void StartAwakeCinema()
    {
        // 카메라를 대화뷰에서 시네뷰로 전환해주고
        //BattleManager.Instance.CineCam(true);

        // 화면 전환 컴포지션 재생(페이드인아웃)
        compositionUI.SetActive(true);
        StartCoroutine(FadeOutAndShowText(1.3f));

        // 시네마UI 활성화
        cinemaUI.SetActive(true);
        MonsterDialogue.SetActive(false);
        // 몬스터 애니메이션 출력
        //BattleManager.Instance.enemyAnim.SetTrigger("Rage"); // 일단 Rage로 테스트

    }

    private IEnumerator FadeOutAndShowText(float duration)
    {
        yield return StartCoroutine(FadeOutAndIn(duration));

        // 대사 출력
        yield return StartCoroutine(TypeText("안녕 나는 발록이야 지금부터 싸우자"));

        // 대기 후 EndAwakeCinema 실행
        yield return StartCoroutine(WaitAndEndCinema(5f));
    }


    // 시네마틱 종료하기 전까지 기다리기
    private IEnumerator WaitAndEndCinema(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        EndAwakeCinema();
    }

    public void EndAwakeCinema()
    {
        // 화면 전환 컴포지션 재생(페이드인아웃)
        StartCoroutine(FadeOutAndIn(1.3f));

        // 시네마UI 끄기
        cinemaUI.SetActive(false);

        // 배틀UI 활성화는 FadeOutAndIn 끝나고 나서
        StartCoroutine(ActivateBattleUIAfterFade());
    }

    private IEnumerator ActivateBattleUIAfterFade()
    {
        // FadeOutAndIn이 끝날 때까지 기다림
        yield return StartCoroutine(FadeOutAndIn(1.3f));

        // 배틀UI 활성화
        battleUI.SetActive(true);
    }

    // 타이핑 효과 함수
    private IEnumerator TypeText(string text)
    {
        MonsterDialogue.GetComponent<TMP_Text>().text = "";
        MonsterDialogue.SetActive(true);

        foreach (char c in text)
        {
            MonsterDialogue.GetComponent<TMP_Text>().text += c;
            yield return new WaitForSeconds(0.1f); // 타이핑 속도
        }
    }

    private IEnumerator Fade(Image img, float start, float end, float duration)
    {
        float elapsed = 0f;
        Color color = img.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(start, end, elapsed / duration);
            img.color = color;
            yield return null;
        }

        color.a = end;
        img.color = color;
    }

    private IEnumerator FadeOutAndIn(float duration)
    {
        // 페이드 시작 전에 compositionUI 활성화
        compositionUI.SetActive(true); 

        yield return StartCoroutine(Fade(backImage, 0, 1, duration / 2));
        yield return StartCoroutine(Fade(backImage, 1, 0, duration / 2));

        // compositionUI 비활성화
        compositionUI.SetActive(false);
    }
}


