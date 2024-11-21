using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//생성할 아바타 데이터 지정
public class AvatarPresetSettings : MonoBehaviour
{
    public static AvatarPresetSettings instance;

    public static AvatarPresetSettings Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/avatar/AvatarPresetSettings");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<AvatarPresetSettings>();

                if (instance == null)
                {
                    Debug.LogError("AvatarPresetSettings 컴포넌트를 찾을 수 없습니다!");
                    return null;
                }
            }
            else
            {
                print("없는데요?");
                return null;
            }
        }
        return instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public struct GenderParts
    {
        [Header("부위별 분류 0: 스킨 / 1: 헤어 / 2: 의류 / 3: 도구")]
        public AvatarParts[] avatarParts;
        [Header("성별 바디 메쉬")]
        public SkinnedMeshRenderer mesh;
    }
    [Space]
    [Header("성별 -> 부위 -> 아이템별 순서")]
    [SerializeField]
    public GenderParts[] genderParts = new GenderParts[] { };

    [System.Serializable]
    public struct AvatarItems
    {
        [Header("더미 데이터")]
        public string name;
        [Header("메인 메쉬")]
        public SkinnedMeshRenderer mesh;
        [Header("보조 메쉬: 의류의 피부 메쉬 처리")]
        public SkinnedMeshRenderer subMesh;
        [Header("더미 데이터")]
        public MeshFilter meshB;
        [Header("메인 메쉬의 메테리얼")]
        public Material material;
        [Header("UI에 표시할 아이콘")]
        public Sprite sprite;
    }

    [System.Serializable]
    public struct AvatarParts
    {
        [Header("itemCard 배열")]
        public AvatarItems[] avatarItems;
    }
}
