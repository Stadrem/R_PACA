using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPresetSettings : MonoBehaviour
{
    public static AvatarPresetSettings instance;

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

    [System.Serializable]
    public struct GenderParts
    {
        public AvatarParts[] avatarParts;
    }

    [Header("성별")]
    public GenderParts[] genderParts;

    [System.Serializable]
    public struct AvatarItems
    {
        public string name;
        public SkinnedMeshRenderer mesh;
        public Material material;
        public Sprite sprite;
    }

    [System.Serializable]
    public struct AvatarParts
    {
        [Header("itemCard 배열")]
        public AvatarItems[] avatarItems;
    }


    /*
    [Header("Content 배열")]
    public AvatarParts[] avatarParts;
    */

}
