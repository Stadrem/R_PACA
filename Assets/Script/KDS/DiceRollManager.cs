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

    public GameObject dicePrefab;
    public GameObject[] diceObjects;
    public int diceCount = 4;
    public List<int> diceResults = new List<int>(); // 주사위 결과 저장 리스트

    // Start is called before the first frame update
    void Start()
    {
        diceObjects = new GameObject[diceCount];

        for (int i = 0; i < diceCount; i++)
        {
            diceObjects[i] = Instantiate(dicePrefab, transform);
            diceObjects[i].transform.parent = transform;
            diceObjects[i].SetActive(false);
            diceObjects[i].GetComponent<DiceNumCall>().checkOut = false;
        }
    }

    public void DiceRoll(int callDiceCount)
    {
        diceResults.Clear(); // 결과 초기화

        for (int i = 0; i < callDiceCount; i++)
        {
            diceObjects[i].SetActive(true);
            diceObjects[i].transform.localPosition = new Vector3(i * 0.05f, 5, 0);
            diceObjects[i].GetComponent<Rigidbody>().AddTorque(new Vector3(30 * Random.Range(1.0f, 2.0f), 20 * Random.Range(1.0f, 2.0f)));
            diceObjects[i].GetComponent<DiceNumCall>().checkOut = false;
            diceObjects[i].GetComponent<DiceNumCall>().onDiceResult = OnDiceResult; // 콜백 연결
        }

        StartCoroutine(CheckDiceResults(callDiceCount));
    }

    // 주사위 굴림 결과를 체크하는 코루틴
    IEnumerator CheckDiceResults(int callDiceCount)
    {
        while (diceResults.Count < callDiceCount)
        {
            yield return null; // 모든 주사위가 멈출 때까지 대기
        }

        int tempDiceResult = diceResults.Sum();

        Debug.Log(callDiceCount + "D" + tempDiceResult); // 결과 출력
    }

    // 각 주사위 결과를 콜백으로 받음
    private void OnDiceResult(int result)
    {
        diceResults.Add(result);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            DiceRoll(2);
        }
    }
}
