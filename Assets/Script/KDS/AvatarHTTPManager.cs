using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpManager;
using UnityEngine.Networking;

public class AvatarHTTPManager : MonoBehaviour
{
    public static AvatarHTTPManager instance;

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
        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = "";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = "";

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 서버로부터 받은 응답 출력
            print(downloadHandler.text);
        };

        HttpManager.GetInstance().GetAvatarInfo(info);
    }

    public MyAvatar myAvatar = new MyAvatar();

    public void GetAvatarCode(int parts, int code)
    {
        switch(parts)
        {
            case 0:
                myAvatar.userAvatarGender = code;
                break;
            case 1:
                myAvatar.userAvatarSkin = code; 
                break;
            case 2:
                myAvatar.userAvatarHair = code;
                break;
            case 3:
                myAvatar.userAvatarBody = code;
                break;
            case 4:
                myAvatar.userAvatarHand = code;
                break;
        }
        
        print("부위" + parts + " 아이템 넘버 " + code);

        AvatarRefresh();
    }

    public void AvatarRefresh()
    {

    }

    public void OnClickAvatarFinish()
    {
        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = "";

        // 전송할 데이터를 JSON 형식으로 변환하여 설정
        info.body = JsonUtility.ToJson(myAvatar);

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            // 서버로부터 받은 응답 출력
            print(downloadHandler.text);
        };

        // POST 요청을 위한 코루틴 실행
        StartCoroutine(HttpManager.GetInstance().SendAvatarInfo(info));
    }
}

[System.Serializable]
public struct MyAvatar
{
    public string userName;
    public string userID;
    public int userAvatarGender; //0이면 남자, 1이면 여자.
    public int userAvatarSkin;
    public int userAvatarHair;
    public int userAvatarBody;
    public int userAvatarHand;
}
