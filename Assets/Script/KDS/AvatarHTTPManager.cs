using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpManager;
using UnityEngine.Networking;
using static PlayerAvatarSetting;
using System;
using UnityEditor.Experimental.GraphView;

public class AvatarHTTPManager : MonoBehaviour
{
    public bool CheckConnect()
    {
        return true;
    }

    //서버에 아바타 정보 업로드
    public void StartPutAvatarInfo(MyAvatar myAvatar)
    {
        //MyAvatar postMyAvatar = myAvatar;

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"http://125.132.216.190:8765/user/update?userCode={myAvatar.userCode}&userAvatarGender={myAvatar.userAvatarGender}&userAvatarHair={myAvatar.userAvatarHair}&userAvatarBody={myAvatar.userAvatarBody}&userAvatarSkin={myAvatar.userAvatarSkin}&userAvatarHand={myAvatar.userAvatarHand}";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        //info.body = JsonUtility.ToJson(postMyAvatar);

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text + "아바타 데이터 업로드 성공");
        };

        StartCoroutine(PutAvatarInfo(info));
    }

    //서버에서 아바타 정보 받아오기
    public void StartGetAvatarInfo(int id, Action<MyAvatar> onAvatarReceived)
    {
        //GetMyAvatar getMyAvatar = new GetMyAvatar();

        //getMyAvatar.userCode = id;

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"http://125.132.216.190:8765/user?userCode={id}";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        //info.body = JsonUtility.ToJson(getMyAvatar); ;

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            //아바타 정보를 스크립트에 반환
            string json = downloadHandler.text;

            MyAvatar get = JsonUtility.FromJson<MyAvatar>(json);

            // 콜백 함수로 아바타 정보 반환
            onAvatarReceived?.Invoke(get);

            print("유저 ID: " + id + " 아바타 정보 받아오기 성공");
        };

        StartCoroutine(GetAvatarInfo(info));
    }

    //아바타 정보 서버에 저장
    public IEnumerator PutAvatarInfo(HttpInfo info)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "Put"))
        {
            //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            //webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            //webRequest.SetRequestHeader("Content-Type", info.contentType);

            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                info.onComplete(webRequest.downloadHandler);
            }
            else
            {
                Debug.Log("failed: " + webRequest.error);
            }
        }
    }

    //아바타 정보 가져오기
    public IEnumerator GetAvatarInfo(HttpInfo info)
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

[System.Serializable]
public struct GetMyAvatar
{
    public int userCode;
}

//현재 아바타 세팅 저장
[System.Serializable]
public struct MyAvatar
{
    public int userCode;
    public int userAvatarGender; //0이면 남자, 1이면 여자.
    public int userAvatarHair;
    public int userAvatarBody;
    public int userAvatarSkin;
    public int userAvatarHand;
}
