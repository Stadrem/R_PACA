using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

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
                    Debug.LogError("DiceRollManager 컴포넌트를 찾을 수 없습니다!");
                    return null;
                }
            }
            else
            {
                print("없는데요?");
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

        diceObjects = new GameObject[diceCount];

        for (int i = 0; i < diceCount; i++)
        {
            diceObjects[i] = Instantiate(dicePrefab, transform);
            diceObjects[i].transform.parent = transform;
            diceObjects[i].SetActive(false);
        }

      ClearValue();
    }

    //주사위 프리팹
    public GameObject dicePrefab;

    //오브젝트풀 생성
    GameObject[] diceObjects;

    //생성할 주사위 갯수
    public int diceCount = 2;

    // 주사위 결과 저장 리스트
    public List<int> diceResults = new List<int>(); 

    //결과값 합산
    int diceResult;

    public LayerMask layerMask;

    // 레이캐스트가 도달할 최대 거리
    float maxRayDistance = 1000f; 

    //적정값 입력
    float rollDuration = 0.7f; // 주사위가 굴러가는 시간

    public GameObject canvas;

    public TMP_Text diceText;

    public TMP_Text titleText;

    public TMP_Text plusText;

    //코루틴 변수
    private IEnumerator coroutine;

    public Transform createPoint;

    //기본 공격력 값
    int baseAttack = 6;

    public bool autoPosition = true;

    //성공 실패만 판별하는 탐색 전용 주사위
    public bool SearchDiceRoll(int stat)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        //최종 결과값 초기화
        bool result = false;

        titleText.text = "결과: " + "실패";

        //주사위 랜덤 값 + 보정값
        int sumDice = DiceRandomPick() + AbilityCorrection(stat);

        //최종 주사위 값 기반으로 피해량 결정
        //6 이하는 실패
        //7 이상은 성공
        if (sumDice < 7) 
        {
            result = false;
        }

        //성공 실패 여부 표시
        if (result)
        {
            titleText.text = "결과: " + "성공";
        }

        //주사위 굴리기 비주얼
        DiceRollView();

        return result;
    }

    //전투 전용 주사위
    public int BattleDiceRoll(int stat)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        //최종 결과값 초기화
        int result = 0;

        //주사위 랜덤 값 + 보정값
        int sumDice = DiceRandomPick() + AbilityCorrection(stat);

        //최종 주사위 값 기반으로 피해량 결정
        //3 이하는 0%
        //4~6은 50 %
        //7~11은 100 %
        //12 이상은 200 %
        if (sumDice >= 4 && sumDice <= 6)
        {
            result = (int)(baseAttack * 0.5f);
            print("50% 피해!");
        }
        else if (sumDice >= 7 && sumDice <= 11)
        {
            result = baseAttack;
            print("100% 피해!");
        }
        else if (sumDice >= 12)
        {
            result = baseAttack * 2;
            print("200% 피해!");
        }

        titleText.text = "결과: " + result + "공격력";

        //주사위 굴리기 비주얼
        DiceRollView();

        return result;
    }

    //능력치 보정 함수
    int AbilityCorrection(int stat)
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

        plusText.text = "능력치 보정: +" + plusDice;

        return plusDice;
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

    // 단순 굴리기 용도
    public void DiceRoll(int diceA, int diceB)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        diceResults.Add(diceA);
        diceResults.Add(diceB);

        //나온 값 합산
        diceResult = diceResults.Sum();

        titleText.text = "";

        //주사위 굴리기 비주얼
        DiceRollView();
    }


    //백엔드 주사위 굴리기
    public void DiceRoll(int diceA, int diceB, bool result)
    {
        //텍스트랑 오브젝트 초기화
        ClearValue();

        diceResults.Add(diceA);
        diceResults.Add(diceB);

        //나온 값 합산
        diceResult = diceResults.Sum();

        titleText.text = "결과: " + "실패";

        //성공 실패 여부 표시
        if (result)
        {
            titleText.text = "결과: " + "성공";
        }

        //주사위 굴리기 비주얼
        DiceRollView();
    }


    //int 값으로 반환
    public void DiceRollView() 
    {
        // 카메라 중심에서 레이캐스트 발사
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit hit; // 충돌 정보를 저장할 변수

        //오토 포지션 체크 되어있으면, 카메라 위치 변경때마다 위치 갱신, 아니라면, 첫 위치 그대로 생성
        if (autoPosition)
        {
            // 레이캐스트가 충돌했는지 확인
            if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
            {
                // 충돌 지점의 월드 좌표
                Vector3 hitPoint = hit.point;

                transform.position = new Vector3(hitPoint.x, hitPoint.y + 0.5f, hitPoint.z);
            }
            else
            {
                // 레이캐스트가 아무 오브젝트와도 충돌하지 않았을 때
                transform.position = new Vector3(0, 0, 0);
                print("없음");
            }
        }

        //주사위 오브젝트풀 활성화
        for (int i = 0; i < diceCount; i++)
        {
            diceObjects[i].SetActive(true);

            //등장 위치 무작위
            diceObjects[i].transform.localPosition = new Vector3(createPoint.position.x + (i*2), createPoint.position.y, createPoint.position.z * Random.Range(0.8f, 1.1f));

            //회전 값 무작위
            diceObjects[i].GetComponent<Rigidbody>().AddTorque(new Vector3(120 * Random.Range(1.6f, 1.9f), 60 * Random.Range(1.6f, 1.9f)));
        }

        SoundManager.Get().PlaySFX(0);

        print(diceCount + "D" + diceResult);

        diceText.text = diceCount + "D" + diceResult;

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
        yield return new WaitForSeconds(rollDuration);

        canvas.SetActive(true);

        // 주사위의 결과에 맞게 회전 설정
        for (int i = 0; i < diceCount; i++)
        {
            //주사위 나온 값에 맞춰서 최종 회전값 설정
            Rigidbody rb = diceObjects[i].GetComponent<Rigidbody>();
            SetDiceResult(diceResults[i], rb);
        }

        yield return new WaitForSeconds(3);

        for (int j = 0; j < diceCount; j++)
        {
            diceObjects[j].SetActive(false);
        }
    }

    //주사위 회전 함수
    public void SetDiceResult(int result, Rigidbody rb)
    {
        Quaternion finalRotation;

        switch (result)
        {
            //주사위의 눈에 맞는 회전값
            case 1:
                finalRotation = Quaternion.Euler(-90, 0, 0);  
                break;
            case 2:
                finalRotation = Quaternion.Euler(0, 0, 0); 
                break;
            case 3:
                finalRotation = Quaternion.Euler(0, 0, -90); 
                break;
            case 4:
                finalRotation = Quaternion.Euler(0, 0, 90); 
                break;
            case 5:
                finalRotation = Quaternion.Euler(0, 0, -180); 
                break;
            case 6:
                finalRotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                finalRotation = Quaternion.Euler(-90, 0, 0);
                break;
        }

        //물리 회전 멈추기
        rb.angularVelocity = Vector3.zero;

        // 주사위의 최종 위치와 회전 설정
        rb.MoveRotation(finalRotation);
    }

    void ClearValue()
    {
        //저장된 값 초기화
        diceResults.Clear();
        diceResult = 0;

        titleText.text = " ";
        plusText.text = " ";
        diceText.text = " ";

        canvas.SetActive(false);
    }
}
