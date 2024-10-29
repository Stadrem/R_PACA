using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        diceSound = GetComponent<AudioSource>();
    }

    //주사위 프리팹
    public GameObject dicePrefab;

    //오브젝트풀 생성
    GameObject[] diceObjects;

    //생성할 주사위 갯수
    public int diceCount = 2;

    // 주사위 결과 저장 리스트
    List<int> diceResults = new List<int>(); 

    //결과값 합산
    int diceResult;

    //사운드
    AudioSource diceSound;

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

    public bool BattleDiceRoll(int stat)
    {
        bool result = false;

        int diceA = Random.Range(1, 7);
        int diceB = Random.Range(1, 7);
        int plusDice = -2;

        if (stat == 2 || stat == 3)
        {
            plusDice = -1;
        }
        else if(stat == 4 || stat == 5)
        {
            plusDice = 0;
        }
        else if (stat == 6 || stat == 7)
        {
            plusDice = 1;
        }
        else if (stat == 8 || stat == 9)
        {
            plusDice = 2;
        }

        int sumDice = diceA + diceB + plusDice;

        if(sumDice >= 6)
        {
            result = true;
            print(diceA + " " + diceB + "판정 성공!");
        }
        else
        {
            print(diceA + " " + diceB + "판정 실패!");
        }

        plusText.text = "능력치 보정: " + plusDice;

        DiceRoll(diceA, diceB, result);

        return result;
    }

    //int 값으로 반환
    public void DiceRoll(int diceA, int diceB, bool decide) 
    {
        string decideText = "실패";

        // 카메라 중심에서 레이캐스트 발사
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit hit; // 충돌 정보를 저장할 변수

        // 레이캐스트가 충돌했는지 확인
        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
        {
            // 충돌 지점의 월드 좌표
            Vector3 hitPoint = hit.point;

            transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
        }
        else
        {
            // 레이캐스트가 아무 오브젝트와도 충돌하지 않았을 때
            transform.position = new Vector3(0, 0, 0);
            print("없음");
        }

        //저장된 값 초기화
        diceResults.Clear();
        diceResult = 0;

        //주사위 오브젝트풀 활성화
        for (int i = 0; i < diceCount; i++)
        {
            diceObjects[i].SetActive(true);

            //등장 위치 무작위
            diceObjects[i].transform.localPosition = new Vector3(createPoint.position.x + (i*2), createPoint.position.y, createPoint.position.z * Random.Range(0.8f, 1.1f));

            //회전 값 무작위
            diceObjects[i].GetComponent<Rigidbody>().AddTorque(new Vector3(120 * Random.Range(1.6f, 1.9f), 60 * Random.Range(1.6f, 1.9f)));

            //주사위 1개당 주사위 값 생성
            //diceResults.Add(Random.Range(1,7));
        }

        diceResults.Add(diceA);
        diceResults.Add(diceB);

        diceSound.Play();

        //나온 값 합산
        diceResult = diceResults.Sum();

        if (decide)
        {
            decideText = "성공";
        }

        titleText.text = "결과: " + decideText;

        print(diceResults.Count + "D" +diceResult);
        
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = SetDiceResultsAfterRoll(diceCount);

        // 주사위가 굴러가는 동안 회전 값 설정
        StartCoroutine(coroutine);

        //int 값 반환
        //return diceResult;
    }

    // 코루틴으로 주사위 회전값 설정 (일정 시간 후)
    private IEnumerator SetDiceResultsAfterRoll(int diceCount)
    {
        // 주사위가 굴러가는 시간을 기다림
        yield return new WaitForSeconds(rollDuration);

        diceText.text = diceResults.Count + "D" + diceResult;

        canvas.SetActive(true);

        // 주사위의 결과에 맞게 회전 설정
        for (int i = 0; i < diceCount; i++)
        {
            //주사위 나온 값에 맞춰서 최종 회전값 설정
            Rigidbody rb = diceObjects[i].GetComponent<Rigidbody>();
            SetDiceResult(diceResults[i], rb);
        }

        yield return new WaitForSeconds(3);

        canvas.SetActive(false);

        plusText.text = " ";
        diceText.text = " ";

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
}
