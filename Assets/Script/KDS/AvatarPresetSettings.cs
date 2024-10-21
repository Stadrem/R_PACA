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
        public AvatarParts[] avatarParts;
        public SkinnedMeshRenderer mesh;
    }

    [Header("성별")]
    [SerializeField]
    public GenderParts[] genderParts = new GenderParts[] { };

    [System.Serializable]
    public struct AvatarItems
    {
        public string name;
        public SkinnedMeshRenderer mesh;
        public MeshFilter meshB;
        public Material material;
        public Sprite sprite;
    }

    [System.Serializable]
    public struct AvatarParts
    {
        [Header("itemCard 배열")]
        public AvatarItems[] avatarItems;
    }
}
