using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 현재 씬의 이름을 가져옴
        currentScene = SceneManager.GetActiveScene();
    }

    // 씬이 로드될 때마다 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 씬의 이름을 가져옴
        currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == "Town")
        {
            UnlockAchievement(2);
        }
        switch (currentScene.name)
        {
            case "Town":
                UnlockAchievement(2);
                break;
            case "JYP_TotalTestScene":
                UnlockAchievement(1);
                break;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 등록 해제 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [Header("유니티에 저장 된 칭호 데이터")]
    public List<AchievementData> achievements = new List<AchievementData>();
    [Header("장착된 칭호 데이터")]
    public AchievementData equippedAchievement;
    AchievementUiManager achievementUiManager;

    [Header("칭호 캔버스")]
    public GameObject canvas;

    Scene currentScene;

    // Resources 폴더에서 업적 데이터를 자동으로 로드
    public void LoadAchievements()
    {
        // Resources에서 업적 데이터를 로드
        AchievementData[] loadedAchievements = Resources.LoadAll<AchievementData>("scripts/Achievement");
        achievements = new List<AchievementData>(loadedAchievements);

        // index 값 기준으로 정렬
        achievements.Sort((a, b) => a.set.index.CompareTo(b.set.index));

        achievementUiManager = GetComponent<AchievementUiManager>();

        achievementUiManager.CreateCards();
    }

    // 특정 조건 만족 시 업적 해제
    public void UnlockAchievement(int index)
    {
        if (achievements[index].set.index == index && !achievements[index].set.isUnlocked)
        {
            achievements[index].set.isUnlocked = true;
            Debug.Log($"업적 해제: {achievements[index].set.title}");

            achievementUiManager.GetTitle(achievements[index].set.title, achievements[index].set.description);
        }

        achievementUiManager.RefreshCards();
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
            Debug.Log($"업적 장착: {index}번 인덱스: {achievements[index].set.title}");

            achievementUiManager.Equipped();

            ChangeAchievement(equippedAchievement);

            return;
        }
    }

    // 업적 변경 함수
    public void ChangeAchievement(AchievementData newAchievement)
    {
        equippedAchievement = newAchievement;

        // 이벤트 발생
        OnAchievementChanged?.Invoke(newAchievement.set.title, newAchievement.set.index);

        if (UserCodeMgr.Instance != null)
        {
            UserCodeMgr.Instance.title = newAchievement.set.index;
        }
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

        achievementUiManager.RefreshCards();
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