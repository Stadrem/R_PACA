using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFakeServer : MonoBehaviour
{
    public static TempFakeServer instance;

    public MyAvatar myAvatar = new();

    public static TempFakeServer Get()
    {
        if (instance == null)
        {
            // 새로운 게임 오브젝트 생성
            GameObject newInstance = new GameObject("TempFakeServer");

            // TempFakeServer 컴포넌트를 부착
            instance = newInstance.AddComponent<TempFakeServer>();

            if (instance == null)
            {
                Debug.LogError("TempFakeServer 컴포넌트를 추가하는 데 실패했습니다!");
                return null;
            }
        }

        return instance;
    }

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
}
