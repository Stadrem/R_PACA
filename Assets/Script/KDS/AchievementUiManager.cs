using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUiManager : MonoBehaviour
{
    //매니저 받아오기
    private AchievementManager achievementManager;

    [Header("칭호 카드 프리팹")]
    public GameObject titlePrefab;

    [Header("칭호 카드 넣을 콘텐츠 박스")]
    public GameObject contents;

    [Header("생성된 칭호 카드의 스크립트 배열")]
    public List<AchievementCards> achievementCards = new List<AchievementCards>();

    [Header("선택된 칭호의 데이터")]
    public AchievementSet selectAchievement;

    [Header("선택된 칭호를 [선택 중]")]
    public AchievementCards selectCard;

    [Header("칭호 획득 알림 관련")]
    public TMP_Text Text_GetTitleName;

    public TMP_Text Text_GetTitleDesc;

    public GameObject Canvas_GetTitle;

    //칭호 UI 카드 생성
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
            GameObject obj = Instantiate(titlePrefab);

            obj.transform.SetParent(contents.transform, false);

            //스크립트를 배열에 삽입
            achievementCards.Add(obj.GetComponent<AchievementCards>());

            //스크립터블 오브젝트 데이터 가져오기
            achievementCards[i].set = achievementManager.achievements[i].set;

            //언락인지 아닌지 확인
            achievementCards[i].SetUp(achievementCards[i].set.isUnlocked);

            //활성화 칭호 찾기
            if (achievementCards[i].set.isEquipped)
            {
                achievementManager.EquipAchievement(i);
            }

            //버튼에 액션 할당
            int tempindex = achievementCards[i].set.index;
            obj.GetComponent<Button>().onClick.AddListener(() => OnClickTitleChange(tempindex));
        }

        //알 수 없는 이유로 장착 가능한게 없으면 [없음]으로 설정
        if(achievementManager.equippedAchievement == null)
        {
            achievementManager.EquipAchievement(0);
        }
    }

    //카드 정보 갱신
    public void RefreshCards()
    {
        //생성되어있는 요소들 만큼 정보 갱신
        for (int i = 0; i < achievementManager.achievements.Count; i++)
        {
            achievementCards[i].set = achievementManager.achievements[i].set;

            achievementCards[i].SetUp(achievementCards[i].set.isUnlocked);
        }
    }

    //버튼에 반영할 클릭 이벤트
    public void OnClickTitleChange(int num)
    {
        //매니저가 가지고 있는 장착 이벤트 설정
        achievementManager.EquipAchievement(num);
    }

    //[선택 중] 갱신 함수
    public void Equipped()
    {
        //매니저에서 장착된 칭호 찾아서, 선택중에다가 갱신
        selectCard.set = achievementManager.GetEquippedAchievement().set;

        selectCard.SetUp(true);
    }

    //업적 획득 알림 타이틀
    public void GetTitle(string titlename, string desc)
    {
        StartCoroutine(CoGetTitle(titlename, desc));
    }

    //업적 획득 알림 타이틀 코루틴
    public IEnumerator CoGetTitle(string titlename, string desc)
    {
        //칭호 획득 효과음
        SoundManager.Get().PlaySFX(7);

        //칭호 이름 가져오기
        Text_GetTitleName.text = "~ " + titlename + " ~";

        //칭호 설명 가져오기
        Text_GetTitleDesc.text =  desc + " 달성!";

        //UI 활성화
        Canvas_GetTitle.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        //UI 비활성화
        Canvas_GetTitle.SetActive(false);
    }
}