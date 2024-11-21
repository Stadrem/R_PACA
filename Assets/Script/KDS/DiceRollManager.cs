using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


//주사위의 카메라 생성 위치는 y -200에 있습니다.
public class DiceRollManager : MonoBehaviour
{
    //싱글톤
    public static DiceRollManager instance;

    public static DiceRollManager Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/dice/DiceRollManager");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);

                instance = newInstance.GetComponent<DiceRollManager>();

                if (instance == null)
                {
                    Debug.LogError("DiceRollManagerV2 컴포넌트를 찾을 수 없습니다!");
                    return null;
                }
            }
            else
            {
                print("걍 없는데요?");
                return null;
            }
        }
        return instance;
    }

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

        //new 반복 안하려고 미리 선언
        ws = new WaitForSeconds(spinTime);

        NewDiceSetup(diceCount);
    }

    /// <summary>
    /// 주사위 생성 및 갱신 함수. 이미 생성되어있더라도, int 값을 변경하여 호출하면 재생성됨.
    /// </summary>
    /// <param name="count">생성하고 싶은 주사위 갯수</param>
    public void NewDiceSetup(int count)
    {
        diceCount = count;

        //혹시라도 값이 있으면 파괴
        if (diceObjects != null)
        {
            for (int i = 0; i < diceObjects.Length; i++)
            {
                Destroy(diceObjects[i]);
            }
        }

        //배열 크기 선언
        diceObjects = new GameObject[count];
        diceSpin = new DiceSpin[count];

        //오브젝트 풀 생성
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                //주사위 프리팹 가져오기
                diceObjects[i] = Instantiate(dicePrefab, transform);

                //자식으로 만들기
                diceObjects[i].transform.parent = transform;

                //스크립트 가져오기
                diceSpin[i] = diceObjects[i].GetComponent<DiceSpin>();

                //회전 시간 동기화
                diceSpin[i].GetSettings(spinTime);

                //생성 위치
                diceObjects[i].transform.localPosition = new Vector3(createPoint.position.x + i * 1.5f, 0, 0);

                //만들었으니 감춰
                diceObjects[i].SetActive(false);
            }

            // 카메라를 주사위 중심으로 이동
            AdjustCameraToCenter(diceObjects, renderCamera);
        }
        ClearValue();

        if (inRenderTextureActive)
        {
            diceRT.enabled = true;
        }
    }

    //렌더 텍스처 카메라의 위치를 주사위들의 위치 중심으로 설정
    void AdjustCameraToCenter(GameObject[] diceObjects, Camera camera)
    {
        float centerX = GetCenterX(diceObjects);

        // 카메라의 새로운 위치 설정
        Vector3 newCameraPosition = new Vector3(centerX, camera.transform.localPosition.y, camera.transform.localPosition.z);
        camera.transform.localPosition = newCameraPosition;
    }

    //주사위 전용 카메라 위치 설정
    float GetCenterX(GameObject[] diceObjects)
    {
        float totalX = 0;
        int activeCount = 0;

        foreach (var dice in diceObjects)
        {
            totalX += dice.transform.position.x;
            activeCount++;
        }

        return activeCount > 0 ? totalX / activeCount : 0;
    }

    [Space]
    [Header("생성할 주사위 프리팹")]
    public GameObject dicePrefab;

    //오브젝트풀 생성
    GameObject[] diceObjects;

    //주사위의 회전 스크립트
    DiceSpin[] diceSpin;

    [Space]
    [Header("등장 주사위 갯수")]
    public int diceCount = 2;

    [Space]
    [Header("주사위 회전 시킬 시간")]
    [Tooltip("각 주사위의 회전에도 자동적으로 값 대입됨.")]
    public float spinTime = 1.5f;

    //new 생성 안하려고 고정시켜놓음
    WaitForSeconds ws;

    [Space]
    [Header("주사위 랜덤 연산 후, 결과 값 저장 변수")]
    [Tooltip("주사위 개당 1개씩 배열 변수")]
    public List<int> diceResults = new List<int>(); 

    //결과값 합산
    int diceResult;

    [Space]
    [Header("결과창 캔버스")]
    public GameObject canvas;
    public TMP_Text titleText;
    public TMP_Text plusText;

    //코루틴 변수
    private IEnumerator coroutine;

    [Space]
    [Header("주사위가 생성될 로컬 포지션")]
    //주사위 생성 위치
    public Transform createPoint;

    [Space]
    [Header("주사위 평균 공격력")]
    [Tooltip("0%, 50%, 100%, 200%")]
    public int baseAttack = 6;
    [Header("최대 데미지")]
    [Tooltip("입력될 값 이상일 시 200% 데미지")]
    public int doubleAttackAbove = 12;
    [Header("평균 데미지")]
    [Tooltip("입력될 값 이하일 시 100% 데미지")]
    public int middleAttackBelow = 11;
    [Tooltip("입력될 값 이상일 시 100% 데미지")]
    public int middleAttackAbove = 7;
    [Header("절반 데미지")]
    [Tooltip("입력될 값 이하일 시 50% 데미지")]
    public int halfAttackBelow = 6;
    [Tooltip("입력될 값 이상일 시 50% 데미지")]
    public int halfAttackAbove = 4;

    [Space]
    [Header("일반, 탐색 주사위 설정값")]
    [Tooltip("설정된 값 이상일 시 성공, 미만일 시 실패")]
    public int searchDiceRoll = 7;

    //박진영이 추가한 콜백 기능..?
    public Action onDiceRollFinished = null;

    //업적 전용 변수
    int failSum = 0;

    //공격 실패 여부 bool
    bool success = false;
    bool critical = false;

    bool diceStandby = false;

    [Space]
    [Header("내장된 렌더 텍스처 캔버스 사용")]
    public bool inRenderTextureActive = false;
    public GameObject cameraCanvas;
    public Camera renderCamera;
    public RawImage diceRT;

    /// <summary>
    /// Bool 반환, 성공 실패만 판별하는 탐색 전용 주사위 + 자체 계산 포함
    /// <para>최종 주사위 값 기반으로 피해량 결정</para>
    /// <para>6 이하는 실패</para>
    /// <para>7 이상은 성공</para>
    /// <para>능력치 int 값 입력, 싫으면 -1 입력</para>
    /// </summary>
    /// <param name="stat">연산할 능력치, 능력치 없으면 -1 입력</param>
    /// <returns></returns>
    public bool SearchDiceRoll(int stat)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        //랜덤 결과 연산
        int dicePick = DiceRandomPick();

        //주사위 랜덤 값 + 능력치 보정값
        int sumDice;
        sumDice = dicePick + AbilityCorrection(stat, dicePick);

        //성공 실패 여부
        if (sumDice >= searchDiceRoll)
        {
            success = true;

            titleText.text = "<size=75><color=red>성공</color></size>";

            //업적 달성 전용
            failSum++;
            if (failSum == 3)
            {
                //필연적인
                AchievementManager.Get().UnlockAchievement(10);
                failSum = 0;
            }
        }
        else
        {
            success = false;

            titleText.text = "<size=75><color=blue>실패</color></size>";

            //업적 달성 전용
            failSum--;
            if (failSum == -3)
            {
                //아 눌렀다고 업적
                AchievementManager.Get().UnlockAchievement(9);
                failSum = 0;
            }
        }

        //주사위 굴리기 비주얼
        DiceRollView(diceResults);

        //성공 실패 여부 반환
        return success;
    }

    /// <summary>
    /// int 데미지 값을 반환, 전투 전용 주사위  + 자체 계산 포함
    /// <para>최종 주사위 값 기반으로 피해량 결정</para>
    /// <para>3 이하는 0%</para>
    /// <para>4~6은 50 %</para>
    /// <para>7~11은 100 %</para>
    /// <para>12 이상은 200 %</para>
    /// <para>능력치 int 값 입력, 싫으면 -1 입력</para>
    /// </summary>
    /// <param name="stat">연산할 능력치, 능력치 없으면 -1 입력</param>
    /// <returns></returns>
    public int BattleDiceRoll(int stat)
    {
        //텍스트랑 변수, 오브젝트 초기화
        ClearValue();

        //랜덤 결과 연산
        int dicePick = DiceRandomPick();

        //주사위 랜덤 값 + 능력치 보정값
        int sumDice;
        sumDice = dicePick + AbilityCorrection(stat, dicePick);

        //최종 주사위 값 기반으로 피해량 결정
        if (sumDice >= halfAttackAbove && sumDice <= halfAttackBelow)
        {
            //50% 피해
            diceResult = (int)(baseAttack * 0.5f);
            success = false;

            //50% 피해
            titleText.text = "<color=blue>공격력 " + diceResult + "..</color>";
        }
        else if (sumDice >= middleAttackAbove && sumDice <= middleAttackBelow)
        {
            //100% 피해
            diceResult = baseAttack;
            success = true;

            //일반 공격
            titleText.text = "공격력 " + diceResult;
        }
        else if (sumDice >= doubleAttackAbove)
        {
            //200% 피해
            diceResult = baseAttack * 2;
            success = true;
            critical = true;

            //치명타!
            titleText.text = "<size=75><color=red>공격력 " + diceResult + "!</color></size>";

            //힘세고 강한 업적
            AchievementManager.Get().UnlockAchievement(4);
        }
        else
        {
            success = false;

            //공격 완전 실패
            titleText.text = "<size=55><color=blue>" + "공격 실패..." + "!</color></size>";

            //손이 미끄러진 업적
            AchievementManager.Get().UnlockAchievement(6);
        }

        //주사위 굴리기 비주얼
        DiceRollView(diceResults);

        //데미지 값 반환
        return diceResult;
    }

    //능력치 보정 함수
    int AbilityCorrection(int stat, int dicePick)
    {
        //-1은 능력치 없는걸로 판단
        if(stat == -1)
        {
            //보정값 기본값
            int plusDice = 0;

            plusText.text = "주사위: " + dicePick;

            //보정 값 반환!
            return plusDice;
        }
        else
        {
            //보정값 기본값
            int plusDice = -2;

            //함수 int값에 보정할 능력치 입력 (0~9)
            //0~1: -2 / 2~3: -1 / 4~5: 0 / 6~7: +1 / 8이상: +2
            if (stat == 2 || stat == 3)
            {
                plusDice = -1;
            }
            else if (stat == 4 || stat == 5)
            {
                plusDice = 0;
            }
            else if (stat == 6 || stat == 7)
            {
                plusDice = 1;
            }
            else if (stat >= 8)
            {
                plusDice = 2;
            }

            plusText.text = "주사위: " + dicePick + "<size=40><color=red>+" + plusDice + "</color></size>";

            //보정 값 반환!
            return plusDice;
        }
    }

    /// <summary>
    /// 어디선가 값을 가져올때 사용하는 주사위 굴림 함수 1
    /// </summary>
    /// <param name="diceA">1번째 주사위 눈에 띄울 값</param>
    /// <param name="diceB">2번째 주사위 눈에 띄울 값</param>
    public void DiceRoll(int diceA, int diceB)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        diceResults.Add(diceA);
        diceResults.Add(diceB);

        //나온 값 합산
        diceResult = diceResults.Sum();

        titleText.text = "주사위 값";

        plusText.text = diceA + ", " + diceB;

        success = true;

        //주사위 굴리기 비주얼
        DiceRollView(diceResults);
    }

    /// <summary>
    /// 어디선가 값을 가져올때 사용하는 주사위 굴림 함수 2
    /// </summary>
    /// <param name="diceA">1번째 주사위 눈에 띄울 값</param>
    /// <param name="diceB">2번째 주사위 눈에 띄울 값</param>
    /// <param name="result">성공, 실패 여부</param>
    public void DiceRoll(int diceA, int diceB, bool result)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        success = result;

        diceResults.Add(diceA);
        diceResults.Add(diceB);

        //나온 값 합산
        diceResult = diceResults.Sum();

        titleText.text = "실패";

        //성공 실패 여부 표시
        if (success)
        {
            titleText.text = "성공";
        }

        plusText.text = diceA + ", " + diceB;

        //주사위 굴리기 비주얼
        DiceRollView(diceResults);
    }

    /// <summary>
    /// 주사위만 표시하는 기능
    /// </summary>
    public void DiceStandby()
    {
        //diceStandby가 활성화중이지 않을 때, 렌더 텍스처 카메라를 켜고, diceStandby를 활성화
        //diceStandby가 활성화중일 때, 렌더 텍스처 카메라를 끄고, diceStandby를 비활성화
        ClearValue();

        renderCamera.enabled = !diceStandby;

        cameraCanvas.SetActive(!diceStandby);

        //주사위 오브젝트풀 활성화
        for (int i = 0; i < diceCount; i++)
        {
            //오브젝트 활성화
            diceObjects[i].SetActive(!diceStandby);

            diceSpin[i].DiceStop();
        }

        diceStandby = !diceStandby;

        /*
        
        if (!diceStandby)
        {
            ClearValue();

            renderCamera.enabled = true;

            cameraCanvas.SetActive(true);

            diceStandby = true;

            //주사위 오브젝트풀 활성화
            for (int i = 0; i < diceCount; i++)
            {
                //오브젝트 활성화
                diceObjects[i].SetActive(true);

                diceSpin[i].DiceStop();
            }
        }
        else
        {
            ClearValue();

            renderCamera.enabled = false;

            cameraCanvas.SetActive(false);

            diceStandby = false;

            //주사위 오브젝트풀 활성화
            for (int i = 0; i < diceCount; i++)
            {
                //오브젝트 활성화
                diceObjects[i].SetActive(false);

                diceSpin[i].DiceStop();
            }
        }
        */
    }

    //주사위 값 랜덤
    int DiceRandomPick()
    {
        //1~6 무작위값
        for (int i = 0; i < diceCount; i++)
        {
            diceResults.Add(Random.Range(1, 7));
        }

        //나온 값 합산
        return diceResult = diceResults.Sum();
    }

    //코루틴 시작
    public void DiceRollView(List<int> num) 
    {
        //렌더 텍스처 캔버스 켜기
        cameraCanvas.SetActive(true);
        renderCamera.enabled = true;
        if (inRenderTextureActive)
        {
            diceRT.enabled = true;
        }

        //주사위 굴림 효과음 재생
        SoundManager.Get().PlaySFX(0);

        //사보타지 업적
        if (diceResults[0] == 1 && diceResults[1] == 1)
        {
            AchievementManager.Get().UnlockAchievement(11);
        }

        //주사위 오브젝트풀 활성화
        for (int i = 0; i < diceCount; i++)
        {
            //오브젝트 활성화
            diceObjects[i].SetActive(true);

            //주사위에 붙어있는 스핀 함수 활성화
            diceSpin[i].SpinStart(num[i]);
        }

        //코루틴 초기화
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = SetDiceResultsAfterRoll(diceCount);

        // 주사위가 굴러가는 동안 회전 값 설정
        StartCoroutine(coroutine);
    }

    // 코루틴으로 주사위 회전값 설정 (일정 시간 후)
    private IEnumerator SetDiceResultsAfterRoll(int diceCount)
    {
        // 주사위가 굴러가는 시간을 기다림
        yield return ws;

        //결과창 UI 켜기
        canvas.SetActive(true);

        //박진영 콜백 기능..?
        onDiceRollFinished?.Invoke();

        //효과음 재생
        if (success && critical)
        {
            SoundManager.Get().PlaySFX(9, 0.5f);
        }
        else if (success)
        {
            SoundManager.Get().PlaySFX(4, 0.3f);
        }
        else
        {
            SoundManager.Get().PlaySFX(8, 0.3f);
        }

        // 종료 대기
        yield return ws;

        diceStandby = false;

        // 마무리
        ClearValue();

        for (int j = 0; j < diceCount; j++)
        {
            diceObjects[j].SetActive(false);
        }
    }

    //청소기
    void ClearValue()
    {
        //diceStandby가 활성화 중이지 않을 때만 초기화
        //diceStandby가 준비중일 때 갑자기 렌더 텍스처 꺼지는거 방지하기 위함임
        if (!diceStandby)
        {
            //저장된 변수 초기화
            diceResults.Clear();
            diceResult = 0;
            success = false;
            critical = false;

            //ui 초기화
            titleText.text = " ";
            plusText.text = " ";
            canvas.SetActive(false);
            cameraCanvas.SetActive(false);
            renderCamera.enabled = false;
        }
    }
}