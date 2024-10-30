using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class YarnHistoryCard : MonoBehaviour
{
    public Image backgroundColor;
    public TMP_Text text_User;
    public TMP_Text text_Date;
    public TMP_Text text_Time;
    public TMP_Text text_Title;
    public int historyCode = 0;

    GameObject parent;

    private void Start()
    {
        parent = transform.parent.gameObject;
    }

    public void HistroyInfoSetup(string title, string date, string user, string time, int code)
    {
        text_Title.text = "제목: " + title;
        text_Date.text = "생성일자: " + date;
        text_Time.text = "플레이타임: " + time;
        text_User.text = "참여자: " + user;
        historyCode = code;

        if(historyCode == -1)
        {
            text_Title.text = "";
            text_Date.text = "";
            text_Time.text = "";
            text_User.text = "";
            backgroundColor.color = Color.black;
        }
    }

    public void OnClickYarnPopup()
    {
        StartGetYarnContentInfo(historyCode);
    }

    public void StartGetYarnContentInfo(int id)
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

            parent.GetComponent<YarnHistoryGetSet>().YarnContentPopup();
        };

        StartCoroutine(GetCardContentInfo(info));
    }

    public IEnumerator GetCardContentInfo(HttpInfo info)
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
            }
        }
    }
}