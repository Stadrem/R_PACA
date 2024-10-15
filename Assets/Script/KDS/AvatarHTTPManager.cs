using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpManager;
using UnityEngine.Networking;
using static PlayerAvatarSetting;

public class AvatarHTTPManager : MonoBehaviour
{
    public static AvatarHTTPManager instance;

    public GameObject player;

    PlayerAvatarSetting pas;

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스 설정
            instance = this;

            //씬 전환 시 객체 파괴 방지
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 현재 객체를 파괴
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        
    }

    void FindIsMinePlayer()
    {
        pas = player.GetComponent<PlayerAvatarSetting>();
    }

    public void AvatarRefresh(MyAvatar post)
    {
        pas.myAvatar = post;

        pas.ChangeAvatar();
    }

    //서버에 아바타 정보 업로드
    public void StartPostAvatarInfo()
    {
        MyAvatar postMyAvatar = pas.myAvatar;

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = "";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = JsonUtility.ToJson(postMyAvatar); ;

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            print(downloadHandler.text + "아바타 데이터 업로드 성공");
        };

        StartCoroutine(PostAvatarInfo(info));
    }

    //서버에서 아바타 정보 받아오기
    public void StartGetAvatarInfo(string id)
    {
        GetMyAvatar getMyAvatar = new GetMyAvatar();

        getMyAvatar.userID = id;

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = "";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = JsonUtility.ToJson(getMyAvatar); ;

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            //아바타 정보를 스크립트에 반환
            string json = downloadHandler.text;

            MyAvatar post = JsonUtility.FromJson<MyAvatar>(json);

            AvatarRefresh(post);
        };

        StartCoroutine(GetAvatarInfo(info));
    }

    //아바타 정보 서버에 저장
    public IEnumerator PostAvatarInfo(HttpInfo info)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);

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
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
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
    public string userID;
}
