using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCodeMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static UserCodeMgr Instance { get; private set; }

    // 저장할 userCode
    public int UserCode;

    private void Awake()
    {
        // 싱글톤 인스턴스가 이미 존재한다면 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 다른 씬에서도 파괴되지 않음
        }
    }

    // userCode를 설정하는 함수
    public void SetUserCode(int code)
    {
        UserCode = code;
        Debug.Log("UserCode " + UserCode + "가 저장되었습니다.");
    }
}