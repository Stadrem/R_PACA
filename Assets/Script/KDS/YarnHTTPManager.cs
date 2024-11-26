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

    private string url = "";//URLImport.URL;

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
                yarnCard[0].GetComponent<YarnHistoryCard>().HistroyInfoSetup("이세계 모험가", "24.11.18", "김동수", "00:05:00", - 2);
            }
        }
    }

    public void SimpleTest()
    {
        yarnContentText.text = "세계관 제목: 이세계 모험가\r\n\r\n탐험가: 김동수\r\n\r\n생성 날짜: 24.11.28\r\n\r\n플레이타임: 00:05:00\r\n\r\n내용: \r\n\r김동수: \"안녕하세요 간달프\"\r\n\r\n간달프: \"안녕하시오, 용맹한 모험자여. 그대의 인사는 참으로 따뜻한데다 귀중하군요. 나는 메타 마을의 지혜로운 수호자, 간달프라 하오. 여기서 나는 그대와 같은 용감한 이들을 시험하며, 발록의 던전 비밀을 풀 열쇠를 찾을 수 있도록 돕고 있소. 이미 시험에 응하기로 하였다니, 그대의 용기를 높이 사겠소. 이제 그대의 지혜와 용기를 시험할 차례로군요.\"\r\n\r\n김동수: \"당신의 시험에 도전하겠습니다.\"\r\n\r\n간달프: \"아, 기특하도다! 그대의 결단력과 용기는 참으로 대단하군요. 이제 나의 시험에 도전하여 그대의 지혜와 용맹함을 증명할 기회를 드리겠소. 시험이 성공적으로 마무리된다고 하니, 그대는 발록의 던전이 위치한 메타 마을 북쪽의 비밀을 알 권리를 얻게 될 것이오. 자, 그대의 지혜로움을 펼쳐보이길 바라오. 성공을 빌고 있으니, 신중하게 시험에 임하시오.\"\r\n\r\n김동수:“제가 발록을 퇴치하겠습니다.“ \r\n\r\n김동수는 던전을 탐험하던 중, 던전 끝에는 잠겨있는 문이 있었습니다.\r\n\r\n김동수: \"잠긴 문을 열어보겠습니다.\"\r\n김동수는 잠금해제를 시도했고, 아슬아슬하게 성공했습니다.\r\n\r\n잠긴 문이 열리고, 던전 마지막 방에는 무섭게 생긴 발록이 그를 기다리고 있었습니다.\r\n\r\n김동수는 각오를 다지고 발록과 전투에 나섭니다.  \r\n\r\n치열한 전투 끝에 발록이 쓰러졌고, 마을은 평화를 되찾게 되었습니다.\r\n\r\n끝";

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
