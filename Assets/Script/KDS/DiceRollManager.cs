using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceRollManager : MonoBehaviour
{
    public static DiceRollManager instance;

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

    //주사위 프리팹
    public GameObject dicePrefab;

    //오브젝트풀 생성
    GameObject[] diceObjects;

    //등장할 주사위 갯수
    public int diceCount = 4;

    // 주사위 결과 저장 리스트
    List<int> diceResults = new List<int>(); 

    //결과값 합산
    int diceResult;

    //사운드
    AudioSource diceSound;

    //적정값 입력
    float rollDuration = 0.8f; // 주사위가 굴러가는 시간

    // Start is called before the first frame update
    void Start()
    {
        diceObjects = new GameObject[diceCount];

        for (int i = 0; i < diceCount; i++)
        {
            diceObjects[i] = Instantiate(dicePrefab, transform);
            diceObjects[i].transform.parent = transform;
            diceObjects[i].SetActive(false);
        }

        diceSound = GetComponent<AudioSource>();
    }

    //int 값으로 반환
    public int DiceRoll(int callDiceCount) 
    { 
        //저장된 값 초기화
        diceResults.Clear();
        diceResult = 0;

        //주사위 오브젝트풀 활성화
        for (int i = 0; i < callDiceCount; i++)
        {
            diceObjects[i].SetActive(true);

            //등장 위치 무작위
            diceObjects[i].transform.localPosition = new Vector3(i * 1.5f, 5, Random.Range(-1.1f, 1.1f));

            //회전 값 무작위
            diceObjects[i].GetComponent<Rigidbody>().AddTorque(new Vector3(120 * Random.Range(1.0f, 2.0f), 30 * Random.Range(1.0f, 2.0f)));

            //주사위 1개당 주사위 값 생성
            diceResults.Add(Random.Range(1,7));
        }

        diceSound.Play();

        //나온 값 합산
        diceResult = diceResults.Sum();

        print(diceResults.Count + "D" +diceResult);

        // 주사위가 굴러가는 동안 회전 값 설정
        StartCoroutine(SetDiceResultsAfterRoll(callDiceCount));

        //int 값 반환
        return diceResult;
    }

    // 코루틴으로 주사위 회전값 설정 (일정 시간 후)
    private IEnumerator SetDiceResultsAfterRoll(int callDiceCount)
    {
        // 주사위가 굴러가는 시간을 기다림
        yield return new WaitForSeconds(rollDuration);

        // 주사위의 결과에 맞게 회전 설정
        for (int i = 0; i < callDiceCount; i++)
        {
            //주사위 나온 값에 맞춰서 최종 회전값 설정
            Rigidbody rb = diceObjects[i].GetComponent<Rigidbody>();
            SetDiceResult(diceResults[i], rb);
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            int result = DiceRoll(4);
            Debug.Log("반환된 주사위 결과: " + result);
        }
    }
}
