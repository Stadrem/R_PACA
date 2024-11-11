using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.TextCore.Text;

public class BattleAni : MonoBehaviour
{
    public static BattleAni instance;

    public float lifetime;  // 턴 지속 시간
    public GameObject guideUI;
    public GameObject nextTurnUI;

    public GameObject player;
    public Animator playerAnim;

    public GameObject enemy;
    public Animator enemyAnim;

    public Slider playerHPBar;
    public Slider enemyHPBar;

    public CircularSlider circularSlider;
    public TMP_Text currentTurnTXT;  // 현재 턴 수 표시
    public TMP_Text selectTurnTXT;   // 현재 턴 수 표시
    private int turnCount = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //void Start()
    //{
    //    FindPlayerAndEnemy();

    //    playerHPBar.value = playerHPBar.maxValue; // 플레이어 HP 100%
    //    enemyHPBar.value = enemyHPBar.maxValue; // 몬스터 HP 100%


    //}

    //void Update()
    //{
    //    if (player == null || enemy == null)
    //    {
    //        FindPlayerAndEnemy();
    //    }
    //}

    //void FindPlayerAndEnemy()
    //{
    //    if (player == null)
    //    {
    //        player = GameObject.FindGameObjectWithTag("Player");
    //        if (player != null)
    //        {
    //            playerAnim = player.GetComponentInChildren<Animator>();
    //        }
    //    }

    //    if (enemy == null)
    //    {
    //        enemy = GameObject.FindGameObjectWithTag("Enemy");
    //        if (enemy != null)
    //        {
    //            enemyAnim = enemy.GetComponent<Animator>();
    //        }
    //    }
    //}
    //public void UpdatePlayerHP(float damage)
    //{
    //    if (playerHPBar != null)
    //    {
    //        playerHPBar.value -= damage; // 플레이어 HP 감소
    //        if (playerHPBar.value <= 0)
    //        {
    //            Debug.Log("플레이어가 사망했습니다.");
    //        }
    //    }
    //}

    //public void UpdateEnemyHP(float damage)
    //{
    //    if (enemyHPBar != null)
    //    {
    //        enemyHPBar.value -= damage; // 적 HP 감소
    //        if (enemyHPBar.value <= 0)
    //        {
    //            Debug.Log("적이 사망했습니다.");
    //        }
    //    }
    //}

    //public void Turn01()
    //{
    //    print("1턴 전투 진행 중");
    //    guideUI.SetActive(true);
    //    StartCoroutine(TurnRoutine01());
    //}

    //public void Turn02()
    //{
    //    print("2턴 전투 진행 중");
    //    guideUI.SetActive(true);
    //    StartCoroutine(TurnRoutine02());
    //}

    //public void Turn03()
    //{
    //    print("3턴 전투 진행 중");
    //    guideUI.SetActive(true);
    //    StartCoroutine(TurnRoutine03());
    //}

    //private IEnumerator TurnRoutine01()
    //{
    //    ButtonOff();
    //    float currentTime = 0.0f;
    //    while (currentTime < lifetime)
    //    {
    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    guideUI.SetActive(false);
    //    DiceRollManager.Get().DiceRoll(6, 6, true);

    //    yield return new WaitForSeconds(3.0f);
    //    enemyAnim.SetTrigger("IsDamaged");
    //    UpdateEnemyHP(0.5f);  // 적에게 50데미지

    //    print("1턴 전투 끝");
    //    yield return new WaitForSeconds(2.0f);
    //    nextTurnUI.SetActive(true);
    //    yield return new WaitForSeconds(1.5f);
    //    nextTurnUI.SetActive(false);
    //    circularSlider.ResetSlider();
    //    turnCount++;
    //    UpdateTurnText();
    //    ButtonOn();
    //}

    //private IEnumerator TurnRoutine02()
    //{
    //    ButtonOff();
    //    float currentTime = 0.0f;
    //    while (currentTime < lifetime)
    //    {
    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    guideUI.SetActive(false);
    //    DiceRollManager.Get().DiceRoll(1, 1, false);

    //    yield return new WaitForSeconds(3.0f);
    //    enemyAnim.SetTrigger("IsAttack");
    //    UpdatePlayerHP(0.5f);  // 플레이어 50데미지

    //    print("2턴 전투 끝");
    //    yield return new WaitForSeconds(2.0f);
    //    nextTurnUI.SetActive(true);
    //    yield return new WaitForSeconds(1.5f);
    //    nextTurnUI.SetActive(false);
    //    circularSlider.ResetSlider();
    //    turnCount++;
    //    UpdateTurnText();
    //    ButtonOn();
    //}

    //private IEnumerator TurnRoutine03()
    //{
    //    ButtonOff();
    //    float currentTime = 0.0f;
    //    while (currentTime < lifetime)
    //    {
    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    guideUI.SetActive(false);
    //    DiceRollManager.Get().DiceRoll(5, 4, true);

    //    yield return new WaitForSeconds(3.0f);
    //    enemyAnim.SetTrigger("IsDie");
    //    UpdateEnemyHP(0.5f);   // 적에게 50 데미지 (0됨)

    //    print("3턴 전투 끝, 고블린 퇴치");

    //    yield return new WaitForSeconds(3.5f);
    //    Ending.Get().EnableCanvas();
    //    turnCount++;
    //}

    //private void UpdateTurnText()
    //{
    //    currentTurnTXT.text = $"전투 {turnCount}턴";
    //    selectTurnTXT.text = "<color=#FF0000><b>공격</b></color> / <color=#0000FF><b>방어</b></color>를 선택하세요.";
    //    circularSlider.StartDepletion();
    //}

    //public void ButtonOff()
    //{
    //    TurnCheckSystem.instance.attackButton.interactable = false;
    //    TurnCheckSystem.instance.defendButton.interactable = false;
    //    TurnCheckSystem.instance.turnCompleteButton.interactable = false;
    //}
    //public void ButtonOn()
    //{
    //    TurnCheckSystem.instance.attackButton.interactable = true;
    //    TurnCheckSystem.instance.defendButton.interactable = true;
    //    TurnCheckSystem.instance.turnCompleteButton.interactable = true;
    //}
}
