using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static HttpManager;

public class YarnHTTPManager : MonoBehaviour
{
    public GameObject yarnCardPrefab;

    public GameObject yarnContent;

    public TMP_Text yarnContentText;

    public TMP_Text yarnContentTitleText;

    GameObject[] yarnCard = new GameObject[6];

    public struct YarnCardInfo
    {
        public int code;
        public string title;
        public string date;
        public string user;
        public string time;
    }

    public struct YarnContentInfo
    {
        public int code;
        public string title;
        public string content;
    }

    private string url = "";

    private void Awake()
    {
        CreateCards();
    }

    private void Start()
    {
        GetYarnCard(1);
    }

    void CreateCards()
    {
        for (int i = 0; i < yarnCard.Length; i++)
        {
            yarnCard[i] = Instantiate(yarnCardPrefab);

            yarnCard[i].transform.SetParent(transform, false);
        }
    }

    public void GetYarnCard(int pageNum)
    {
        int startId = ((pageNum - 1) * yarnCard.Length + 1) + 1;

        for (int i = 0; i < yarnCard.Length; i++)
        {
            yarnCard[i].GetComponent<YarnHistoryCard>().Refresh();

            StartGetYarnInfo(startId, yarnCard[i]);
        }
    }

    //서버에서 모험담 정보 받아오기
    public void StartGetYarnInfo(int id, GameObject instance)
    {

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"{url}/user?userCode={id}";

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            string json = downloadHandler.text;

            YarnCardInfo cardInfo = JsonUtility.FromJson<YarnCardInfo>(json);

            //자식 오브젝트안에 값 할당
            YarnHistoryCard itemCard = instance.GetComponent<YarnHistoryCard>();

            itemCard.HistroyInfoSetup(cardInfo.title, cardInfo.date, cardInfo.user, cardInfo.time, cardInfo.code);
        };

        StartCoroutine(GetCardInfo(info, instance));
    }

    public IEnumerator GetCardInfo(HttpInfo info, GameObject instance)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "Get"))
        {
            //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            //webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);

            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 요청 완료 시 처리
                info.onComplete(webRequest.downloadHandler);
            }
            else
            {
                Debug.Log("failed: " + webRequest.error);

                YarnHistoryCard itemCard = instance.GetComponent<YarnHistoryCard>();

                itemCard.HistroyInfoSetup("", "", "", "", -1);

                // 비상
                yarnCard[0].GetComponent<YarnHistoryCard>().HistroyInfoSetup("제목: 이세계 모험가", "생성일자: 24.11.14", "플레이타임: 00:05:00", "참여자: 유저1, 유저2, 유저3, 유저4", -2);
            }
        }
    }

    public void SimpleTest()
    {
        yarnContentText.text = "세계관 제목: 이세계 모험가\r\n\r\n탐험가: 김동수, JYP테스트\r\n\r\n생성 날짜: 24.11.14\r\n\r\n플레이타임: 00:03:30\r\n\r\n내용: \r\n\r김동수:“안녕하세요.“\r\n\r\n노인:“반갑네 낯선 젊은이여“\r\n노인의 얼굴에는 근심과 걱정이 가득합니다. 노인은 플레이어의 모습을 관찰하고는 그가 모험가라는 사실을 깨닫습니다.\r\n노인:“마을에 큰 위협이 닥쳐왔네. 우리를 도와줄 수 있겠나.”\r\n노인은 플레이어의 대답을 기다립니다.\r\n\r\n테스트유저1:“도와드리겠습니다.”\r\n\r\n노인:“고맙네 젊은이.“\r\n노인은 고개를 숙여 플레이어에게 감사의 인사를 전합니다. 노인은 지팡이로 마을의 끝을 가리킵니다.\r\n노인: “저 곳에 버려진 던전이 있다네. 그곳에 못된 고블린 하나가 우리 마을을 괴롭히고있다네.”\r\n노인은 슬픔이 가득한 눈으로 플레이어를 주시합니디. 노인은 외부인에게 도움을 요청해야하는 이 상황을 부끄럽게 생각하지만, 위기의 처한 마을에서는 별다른 방안이 없습니다.\r\n\r\n테스트유저1:“제가 고블린을 퇴치하겠습니다.“ \r\n\r\n테스트유저1은 던전을 탐험하던 중, 던전 끝에는 잠겨있는 문이 있었습니다.\r\n\r\n테스트유저1: \"잠긴 문을 열어보겠습니다.\"\r\n테스트유저1은 잠금해제를 시도했고, 아슬아슬하게 성공했습니다.\r\n\r\n잠긴 문이 열리고, 던전 마지막 방에는 무섭게 생긴 고블린이 그를 기다리고 있었습니다.\r\n\r\n고블린은 사실 거대한 마왕 고블린이었으며, 테스트유저1은 각오를 다지고 고블린과 전투에 나섭니다.  \r\n\r\n치열한 전투 끝에 고블린이 쓰러졌고, 마을은 평화를 되찾게 되었습니다.\r\n\r\n끝";

        yarnContentTitleText.text = "제목: 이세계 모험가";

        yarnContent.SetActive(true);
    }

    //모험담 컨텐츠 불러오기
    public void StartGetYarnContentInfo(int id)
    {
        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"{url}/user?userCode={id}";

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            string json = downloadHandler.text;

            YarnContentInfo contentInfo = JsonUtility.FromJson<YarnContentInfo>(json);

            yarnContentText.text = contentInfo.content;

            yarnContentTitleText.text = contentInfo.title;

            yarnContent.SetActive(true);
        };

        StartCoroutine(GetCardContentInfo(info));
    }

    public IEnumerator GetCardContentInfo(HttpInfo info)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "Get"))
        {
            //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            //webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);

            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 요청 완료 시 처리
                info.onComplete(webRequest.downloadHandler);
            }
            else
            {
                Debug.Log("failed: " + webRequest.error);

                yarnContentText.text = "불러오기 실패";

                yarnContentTitleText.text = "불러오기 실패";

                yarnContent.SetActive(true);
            }
        }
    }
}
