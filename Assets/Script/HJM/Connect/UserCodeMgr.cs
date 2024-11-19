using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCodeMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static UserCodeMgr Instance { get; private set; }

    // 저장할 userCode, userID, nickname
    public int UserCode;
    public string UserID;
    public string Nickname;
    public int title;

    private void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SoundManager.Get();
    }

    // userCode를 설정하는 함수
    public void SetUserCode(int code)
    {
        UserCode = code;
        Debug.Log("UserCode " + UserCode + "가 저장되었습니다.");
    }

    // userID와 nickname을 설정하는 함수
    public void SetUserInfo(string userId, string nickname)
    {
        UserID = userId;
        Nickname = nickname;
        Debug.Log($"UserID: {UserID}, Nickname: {Nickname}가 저장되었습니다.");
    }
}
