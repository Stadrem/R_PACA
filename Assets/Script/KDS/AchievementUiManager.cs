using ExitGames.Client.Photon.StructWrapping;
using System;
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

    public List<GameObject> cards = new List<GameObject>();

    public List<AchievementCards> achievementCards = new List<AchievementCards>();

    public AchievementSet selectAchievement;

    //[선택 중]을 위한 카드 한 장
    public AchievementCards selectCard;

    public TMP_Text Text_GetTitleName;

    public GameObject Canvas_GetTitle;

    public void CreateCards()
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
            cards.Add(Instantiate(titlePrefab));

            cards[i].transform.SetParent(contents.transform, false);

            achievementCards.Add(cards[i].GetComponent<AchievementCards>());

            achievementCards[i].set = achievementManager.achievements[i].set;

            achievementCards[i].SetUp(achievementCards[i].set.isUnlocked);

            //활성화 칭호 찾기
            if (achievementCards[i].set.isEquipped)
            {
                achievementManager.EquipAchievement(i);
            }

            //버튼에 액션 할당
            Button button = cards[i].GetComponent<Button>();

            int tempindex = achievementCards[i].set.index;

            button.onClick.AddListener(() => OnClickTitleChange(tempindex));
        }

        //알 수 없는 이유로 장착 가능한게 없으면 [없음]으로 설정
        if(achievementManager.equippedAchievement == null)
        {
            achievementManager.EquipAchievement(0);
        }
    }

    private void OnEnable()
    {
        //RefreshCards();
    }

    public void RefreshCards()
    {
        //생성되어있는 요소들 만큼 자식 오브젝트 생성
        for (int i = 0; i < achievementManager.achievements.Count; i++)
        {
            achievementCards[i].set = achievementManager.achievements[i].set;

            achievementCards[i].SetUp(achievementCards[i].set.isUnlocked);
        }
    }

    //버튼에 반영할 클릭 이벤트
    public void OnClickTitleChange(int num)
    {
        achievementManager.EquipAchievement(num);
    }

    //[선택 중] 갱신 함수
    public void Equipped()
    {
        selectCard.set = achievementManager.GetEquippedAchievement().set;

        selectCard.SetUp(true);
    }

    public void GetTitle(string titlename)
    {
        StartCoroutine(CoGetTitle(titlename));
    }

    public IEnumerator CoGetTitle(string titlename)
    {
        SoundManager.Get().PlaySFX(7);

        Text_GetTitleName.text = titlename;

        Canvas_GetTitle.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        Canvas_GetTitle.SetActive(false);
    }
}
