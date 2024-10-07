using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// HttpInfo 클래스 선언: HTTP 요청 관련 정보를 담는 클래스
public class HttpInfo
{
    // 요청할 URL
    public string url = "";

    // 전송할 데이터
    public string body = "";

    // 컨텐츠 타입
    public string contentType = "";

    // 요청이 완료되면 호출될 델리게이트
    public Action<DownloadHandler> onComplete;

    // 요청 후 에러뜨면 호출될 델리게이트
    public Action<string> onError;
}

public class HttpManager : MonoBehaviour
{
    string loginUrl = "http://125.132.216.190:12450/api/auth/login";
    string joinUrl = "http://125.132.216.190:12450/api/auth/signup";

    //싱글톤 생성
    static HttpManager instance;

    // 싱글톤 인스턴스를 반환하는 메소드
    public static HttpManager GetInstance()
    {
        if (instance == null)
        {
            // 새로운 게임 오브젝트 생성
            GameObject go = new GameObject();

            // 이름 설정
            go.name = "HttpManager";

            // HttpManager 컴포넌트를 추가
            go.AddComponent<HttpManager>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스 설정
            instance = this;

            // 씬 전환 시 객체 파괴 방지
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 현재 객체를 파괴
            Destroy(gameObject);
        }
    }

    public IEnumerator Login(HttpInfo info)
    {
            // GET 요청 생성
            using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", info.contentType);

                print("로그인중...");
                // 요청 전송 및 응답 대기
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    ParseUserInfo(webRequest.downloadHandler);

                    //로그인 시작 함수 넣으면 됨.
                }
                else
                {
                    Debug.Log("Login failed: " + webRequest.error);
                    if (webRequest.error == "HTTP/1.1 409 Conflict")
                    {
                        print("비밀번호가 틀렸습니다.");
                    }
                    else
                    {
                    print("아이디 혹은 비밀번호가 틀렸습니다.");
                    }
                }
            }
    }

    public IEnumerator Register(HttpInfo info)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);

            print("회원 가입 중...");

            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Registration successful: " + webRequest.downloadHandler.text);
                print("회원 가입 성공!");
                
                //로그인 화면으로 이동하면 됨.
            }
            else
            {
                Debug.Log("Registration failed: " + webRequest.error);
                if (webRequest.error == "HTTP/1.1 500 Internal Server Error")
                {
                    print("중복된 아이디 입니다.");
                }
                else
                {
                    print("회원 가입 실패");
                }
            }
        }
    }

    // 로그인시 반환 받은 토큰 및 계정 정보를 받아내는 함수
    void ParseUserInfo(DownloadHandler downloadHandler)
    {
        string json = downloadHandler.text;

        AccountSet accountSet = JsonUtility.FromJson<AccountSet>(json);

        InAccount(accountSet.response.userId, accountSet.response.userName);
    }

    //GET : 서버에게 데이터를 조회 요청
    public IEnumerator Get(HttpInfo info)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = UnityWebRequest.Get(info.url))
        {
            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            // 요청 완료 시 처리
            DoneRequest(webRequest, info);
        }
    }

    //Post : 데이터를 서버로 전송
    public IEnumerator Post(HttpInfo info)
    {
        //유니티의 http 통신 기능
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, info.body, info.contentType))
        {
            //서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            //서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    
    // 요청 완료 시 호출되는 메소드
    void DoneRequest(UnityWebRequest webRequest, HttpInfo info)
    {
        //만약에 결과가 정상이라면
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            //응답온 데이터를 요청한 클래스로 보내자.
            if (info.onComplete != null)
            {
                info.onComplete(webRequest.downloadHandler);
            }
        }
        //그렇지 않다면 (Error 라면)
        else
        {
            //Error에 대한 이유 출력
            Debug.LogError("Net Error = " + webRequest.error);
        }
    }

    [System.Serializable]
    public struct Response
    {
        public string userId;
        public string userName;
    }

    [System.Serializable]
    public struct AccountSet
    {
        public bool success;
        public Response response;
        public string error;
    }

    public Response response = new Response();

    //로그인 후 정보 저장
    public void InAccount(string userId, string userName)
    {
        response.userId = userId;
        response.userName = userName;
    }

    // 회원 가입시 보낼 정보
    [System.Serializable]
    public struct UserInfo
    {
        public string userId;
        public string userPassword;
        public string userName;
    }

    //로그인 시 보낼 정보
    [System.Serializable]
    public struct UserAccount
    {
        public string userId;
        public string userPassword;
    }

    public void InAccount(string userName)
    {
        response.userName = userName;
    }

    void JoinJsonConvert()
    {
        // 전송할 데이터 객체 생성
        UserInfo userInfo = new UserInfo();
        //userInfo.userId = 유저 아이디
        //userInfo.userPassword = 비밀번호
        //userInfo.userName = 닉네임;

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = joinUrl;

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = JsonUtility.ToJson(userInfo);

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 서버로부터 받은 응답 출력
            print(downloadHandler.text);
        };

        // POST 요청을 위한 코루틴 실행
        StartCoroutine(Register(info));
    }

    void LoginJsonConvert()
    {
        // 전송할 데이터 객체 생성
        UserAccount accountInfo = new UserAccount();
        //accountInfo.userId = 아이디
        //accountInfo.userPassword = 비밀번호

        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = loginUrl;

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = JsonUtility.ToJson(accountInfo);

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 서버로부터 받은 응답 출력
            print(downloadHandler.text);
        };

        // POST 요청을 위한 코루틴 실행
        StartCoroutine(Login(info));
    }

    /*
    public void JoinFinishClick()
    {

        if (string.IsNullOrEmpty(texttId.text) || string.IsNullOrEmpty(textPassword.text) || string.IsNullOrEmpty(textPassword2.text) || string.IsNullOrEmpty(textName.text) || string.IsNullOrEmpty(textArea.text))
        {
            print("빈 칸을 채워주세요.");
            return;
        }

        if (textPassword.text != textPassword2.text)
        {
            print("비밀번호가 동일하지 않습니다.");
            return;
        }

        JoinJsonConvert();
    }

    public void LoginClick()
    {
        if (string.IsNullOrEmpty(logintId.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            print("아이디 및 비밀번호를 입력해주세요.");
            return;
        }

        LoginJsonConvert();
    }
    */

    /*
    // 파일 업로드를 form-data로 처리하는 코루틴
    public IEnumerator UploadFileByFormData(HttpInfo info)
    {
        //info.data에 있는 파일을 byte 배열로 읽어오자
        byte[] data = File.ReadAllBytes(info.body);

        //data를 MultipartForm으로 세팅
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("file", data, "image.jpg", info.contentType));

        //유니티의 http 통신 기능
        using (UnityWebRequest webRequest = UnityWebRequest.Post(info.url, formData))
        {
            //서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            //서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    //파일 업로드
    public IEnumerator UploadFileByByte(HttpInfo info)
    {
        //info.data에 있는 파일을 byte 배열로 읽어오자
        byte[] data = File.ReadAllBytes(info.body);

        //유니티의 http 통신 기능
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "POST"))
        {
            //업로드하는 데이터
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            webRequest.uploadHandler.contentType = info.contentType;

            //응답받는 데이터 공간
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            //서버에 요청 보내기
            yield return webRequest.SendWebRequest();

            //서버에게 응답이 왔다.
            DoneRequest(webRequest, info);
        }
    }

    //스프라이트 받기
    public IEnumerator DownloadSprite(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(info.url))
        {
            yield return webRequest.SendWebRequest();

            DoneRequest(webRequest, info);
        }
    }

    //오디오 받기
    public IEnumerator DownloadAudio(HttpInfo info)
    {
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(info.url, AudioType.WAV))
        {
            yield return webRequest.SendWebRequest();

            DownloadHandlerAudioClip handler = webRequest.downloadHandler as DownloadHandlerAudioClip;
            //handler.audioClip; 을 Audiosource에 셋팅하고 플레이

            DoneRequest(webRequest, info);
        }
    }
     */
}