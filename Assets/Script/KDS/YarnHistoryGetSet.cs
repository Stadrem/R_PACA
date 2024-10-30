using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static HttpManager;

public class YarnHistoryGetSet : MonoBehaviour
{
    public GameObject yarnCardPrefab;

    public struct YarnCardInfo
    {
        public string title;
        public string date;
        public string user;
        public string time;
        public int code;
    }

    YarnCardInfo cardInfo; 

    public void GetYarnCard()
    {
        // 기존 자식 게임 오브젝트 삭제
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject instance = Instantiate(yarnCardPrefab);

            instance.transform.SetParent(transform, false);

            StartGetYarnInfo(i, instance);
        }
    }

    public void YarnContentPopup()
    {

    }

    //서버에서 모험담 정보 받아오기
    public void StartGetYarnInfo(int id, GameObject instance)
    {
        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"http://125.132.216.190:8765/user?userCode={id}";

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
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
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
}
