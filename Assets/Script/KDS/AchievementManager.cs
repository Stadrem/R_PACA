using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    //싱글톤
    public static AchievementManager instance;

    public static AchievementManager Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/AchievementManager");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<AchievementManager>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
    }

    // 업적 변경 이벤트 정의
    public event System.Action<string, int> OnAchievementChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }

        achievementUiManager = GetComponent<AchievementUiManager>();
    }

    public List<AchievementData> achievements = new List<AchievementData>();
    AchievementData equippedAchievement;
    AchievementUiManager achievementUiManager;

    public GameObject canvas;

    // Resources 폴더에서 업적 데이터를 자동으로 로드
    public void LoadAchievements()
    {
        // Resources에서 업적 데이터를 로드
        AchievementData[] loadedAchievements = Resources.LoadAll<AchievementData>("scripts/Achievement");
        achievements = new List<AchievementData>(loadedAchievements);

        // index 값 기준으로 정렬
        achievements.Sort((a, b) => a.set.index.CompareTo(b.set.index));
    }

    // 특정 조건 만족 시 업적 해제
    public void UnlockAchievement(int index)
    {
        if (achievements[index].set.index == index && !achievements[index].set.isUnlocked)
        {
            achievements[index].set.isUnlocked = true;
            Debug.Log($"업적 해제: {achievements[index].set.title}");
        }
    }

    // 업적 착용
    public void EquipAchievement(int index)
    {
        if (achievements[index].set.isUnlocked)
        {
            // 기존 착용 업적 해제
            if (equippedAchievement != null)
            {
                equippedAchievement.set.isEquipped = false;
            }

            // 새 업적 착용
            equippedAchievement = achievements[index];
            achievements[index].set.isEquipped = true;
            Debug.Log($"업적 장착: {achievements[index].set.title}");

            achievementUiManager.Equipped();

            return;
        }
    }

    // 업적 변경 함수
    public void ChangeAchievement(AchievementData newAchievement)
    {
        equippedAchievement = newAchievement;

        // 이벤트 발생
        OnAchievementChanged?.Invoke(newAchievement.set.title, newAchievement.set.index);
    }

    // 현재 착용 중인 업적 확인
    public AchievementData GetEquippedAchievement()
    {
        return equippedAchievement;
    }

    //캔버스 활성화
    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }
}

[System.Serializable]
public struct AchievementSet
{
    public string title;        // 업적 이름
    public string description; // 업적 설명
    public bool isUnlocked;    // 획득 여부
    public bool isEquipped;    // 착용 여부
    public int index;
    public Color color;
    public Sprite sprite;
}