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
            }
        }
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
