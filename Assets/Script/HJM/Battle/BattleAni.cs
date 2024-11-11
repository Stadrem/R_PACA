using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleAni : MonoBehaviour
{
    //public static BattleAni instance;

    //public GameObject nextTurnUI;
    //public List<GameObject> players;
    //public Animator playerAnim;
    //public GameObject enemy;
    //public Animator enemyAnim;
    //public Slider enemyHPBar;
    //public TMP_Text currentTurnTXT;

    //private int turnCount = 1;

    //public List<GameObject> profiles;
    //private List<ProfileSet> playerProfiles;


    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }

    //    // ProfileSet 컴포넌트 초기화
    //    //playerProfile = player.GetComponent<ProfileSet>();
    //}

    //private void Update()
    //{
    //    if(players != )

    //    if(profiles != null)
    //    {
    //        profiles = TurnCheckSystem.Instance.profiles;
    //        //playerProfiles = profiles.GetComponent<ProfileSet>();
    //    }
    //}

    //// 주사위 공격 성공
    //public void DiceAttackSuccess(int damage)
    //{
    //    enemyAnim.SetTrigger("Hit"); // 몬스터 Hit 트리거
    //    playerAnim.SetTrigger("Attack"); // 플레이어 Attack 트리거
    //    UpdateEnemyHealth(damage); // 몬스터 체력 업데이트
    //    ShowBattleUI("공격 성공!"); // 공격 성공 UI
    //    NextTurn();
    //}

    //// 주사위 공격 실패
    //public void DiceAttackFail()
    //{
    //    enemyAnim.SetTrigger("Defense"); // 몬스터 Defense 트리거
    //    playerAnim.SetTrigger("Attack"); // 플레이어 Attack 트리거
    //    ShowBattleUI("공격 실패"); // 공격 실패 UI
    //    NextTurn();
    //}

    //// 주사위 방어 성공
    //public void DiceDefenseSuccess(int damage)
    //{
    //    enemyAnim.SetTrigger("Attack"); // 몬스터 Attack 트리거
    //    playerAnim.SetTrigger("Defense"); // 플레이어 Defense 트리거
    //    playerProfiles.DamagedPlayer(damage / 2); // 플레이어 체력 감소
    //    ShowBattleUI("방어 성공!"); // 방어 성공 UI
    //    NextTurn();
    //}

    //// 주사위 방어 실패
    //public void DiceDefenseFail(int damage)
    //{
    //    enemyAnim.SetTrigger("Attack"); // 몬스터 Attack 트리거
    //    playerAnim.SetTrigger("Hit"); // 플레이어 Hit 트리거
    //    playerProfile.DamagedPlayer(damage); // 플레이어 체력 감소
    //    ShowBattleUI("방어 실패"); // 방어 실패 UI
    //    NextTurn();
    //}

    //private void UpdateEnemyHealth(int damage)
    //{
    //    enemyHPBar.value = Mathf.Max(enemyHPBar.value - damage, 0); // 적 체력 감소
    //}

    //private void ShowBattleUI(string message)
    //{
    //    currentTurnTXT.text = message;
    //    nextTurnUI.SetActive(true); // 다음턴 UI 표시
    //}

    //private void NextTurn()
    //{
    //    turnCount++;
    //    currentTurnTXT.text = "턴 수: " + turnCount;
    //    StartCoroutine(HideNextTurnUI());
    //}

    //private IEnumerator HideNextTurnUI()
    //{
    //    yield return new WaitForSeconds(2f);
    //    nextTurnUI.SetActive(false);
    //}
}
