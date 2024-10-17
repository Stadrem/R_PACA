using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFakeServer : MonoBehaviour
{
    public static TempFakeServer instance;

    public MyAvatar myAvatar = new();

    public GetMyAvatar info = new();

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
        info.userID = "test";
    }
}
