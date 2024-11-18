using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUiManager : MonoBehaviour
{
    private AchievementManager achievementManager;

    public GameObject titlePrefab;

    public GameObject contents;

    List<GameObject> cards = new List<GameObject>();

    public AchievementSet selectAchievement;

    public AchievementCards achievementCards;

    private void Start()
    {
        achievementManager = GetComponent<AchievementManager>();

        // 기존 자식 게임 오브젝트 삭제
        foreach (Transform child in contents.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //생성되어있는 요소들 만큼 자식 오브젝트 생성
        for (int i = 0; i < achievementManager.achievements.Count; i++)
        {
            GameObject instance = Instantiate(titlePrefab);

            instance.transform.SetParent(contents.transform, false);

            AchievementCards ac = instance.GetComponent<AchievementCards>();

            ac.set = achievementManager.achievements[i].set;

            ac.SetUp();

            //버튼에 액션 할당
            Button button = instance.GetComponent<Button>();

            button.onClick.AddListener(() => OnClickTitleChange(ac.set.index));
        }

        if(UserCodeMgr.Instance != null)
        {
            achievementManager.EquipAchievement(UserCodeMgr.Instance.title);
        }
        else
        {
            //기본값 반영
            achievementManager.EquipAchievement(0);
        }
    }

    //버튼에 반영할 클릭 이벤트
    public void OnClickTitleChange(int num)
    {
        achievementManager.EquipAchievement(num);
    }

    //선택 중 갱신 함수
    public void Equipped()
    {
        achievementCards.set = achievementManager.GetEquippedAchievement().set;

        achievementCards.SetUp();
    }
}
