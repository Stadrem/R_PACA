using UnityEngine;
using System.Collections;

public class BattleAni : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BattleAni instance;

    public float lifetime = 2.0f;  // 턴 지속 시간
    public GameObject guideUI;     // UI 오브젝트

    public GameObject player;
    public Animator playerAnim;

    public GameObject enemy;
    public Animator enemyAnim;

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
                playerAnim = player.GetComponent<Animator>();
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
        guideUI.SetActive(true);  // 턴이 시작될 때 guideUI 활성화
        StartCoroutine(TurnRoutine());  // 코루틴 실행
    }

    private IEnumerator TurnRoutine()
    {
        float currentTime = 0.0f;

        // 지정된 lifetime 동안 대기
        while (currentTime < lifetime)
        {
            currentTime += Time.deltaTime;
            yield return null;  // 다음 프레임까지 대기
        }

        guideUI.SetActive(false);
        DiceRollManager.Get().DiceRoll(6, 6, true); // 1턴 공격 성공값
        print("1턴 전투 끝");

        yield return new WaitForSeconds(2.0f); // 대기
        enemyAnim.SetTrigger("IsAttack");

    }
}
