using UnityEngine;
using System.Collections;

public class BattleAni : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BattleAni instance;

    public float lifetime;  // 턴 지속 시간
    public GameObject guideUI;
    public GameObject nextTurnUI;

    public GameObject player;
    public Animator playerAnim;

    public GameObject enemy;
    public Animator enemyAnim;

    public CircularSlider circularSlider;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Player, Enemy 오브젝트 할당
        FindPlayerAndEnemy();

    }

    void Update()
    {
        // Player와 Enemy를 매 프레임마다 체크하여 존재하지 않을 경우 찾아서 할당
        if (player == null || enemy == null)
        {
            FindPlayerAndEnemy();
        }
    }

    private void FindPlayerAndEnemy()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerAnim = player.GetComponentInChildren<Animator>();
            }
        }

        if (enemy == null)
        {
            enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                enemyAnim = enemy.GetComponent<Animator>();
            }
        }

    }

    public void Turn01()
    {
        print("1턴 전투 진행 중");
        guideUI.SetActive(true);
        StartCoroutine(TurnRoutine01());
    }
    public void Turn02()
    {
        print("2턴 전투 진행 중");
        guideUI.SetActive(true);
        StartCoroutine(TurnRoutine02());
    }
    public void Turn03()
    {
        print("3턴 전투 진행 중");
        guideUI.SetActive(true);
        StartCoroutine(TurnRoutine03());
    }



    private IEnumerator TurnRoutine01()
    {
        float currentTime = 0.0f;

        // 지정된 lifetime 동안 대기
        while (currentTime < lifetime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        guideUI.SetActive(false);
        DiceRollManager.Get().DiceRoll(6, 6, true); // 1턴 공격 성공값

        yield return new WaitForSeconds(2.5f);
        enemyAnim.SetTrigger("IsDamaged");
        print("1턴 전투 끝");
        yield return new WaitForSeconds(1.5f);
        nextTurnUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        nextTurnUI.SetActive(false);
        circularSlider.ResetSlider();

    }
    private IEnumerator TurnRoutine02()
    {
        float currentTime = 0.0f;

        // 지정된 lifetime 동안 대기
        while (currentTime < lifetime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        guideUI.SetActive(false);
        DiceRollManager.Get().DiceRoll(1, 1, false); // 2턴 공격 실패값

        yield return new WaitForSeconds(2.5f);
        enemyAnim.SetTrigger("IsAttack");
        print("2턴 전투 끝");
        yield return new WaitForSeconds(2.0f);
        nextTurnUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        nextTurnUI.SetActive(false);
        circularSlider.ResetSlider();

    }
    private IEnumerator TurnRoutine03()
    {
        float currentTime = 0.0f;

        // 지정된 lifetime 동안 대기
        while (currentTime < lifetime)
        {
            currentTime += Time.deltaTime;
            yield return null;  //
        }

        guideUI.SetActive(false);
        DiceRollManager.Get().DiceRoll(5, 4, true); // 3턴 공격 성공값

        yield return new WaitForSeconds(2.5f);
        enemyAnim.SetTrigger("IsDie");
        print("3턴 전투 끝, 고블린 퇴치");
        yield return new WaitForSeconds(3.5f);
        Ending.Get().EnableCanvas();

    }
}
