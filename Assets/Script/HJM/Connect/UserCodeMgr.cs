using System.Collections;
using UnityEngine;

public class UserCodeMgr : MonoBehaviour
{
    public static UserCodeMgr Instance { get; private set; }
    public int UserCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // userCode를 설정하는 함수
    public void SetUserCode(int code)
    {
        UserCode = code;
        Debug.Log("UserCode " + UserCode + "가 저장되었습니다.");
    }

    // userCode를 반환
    public int GetUserCode()
    {
        return UserCode;
    }
}
