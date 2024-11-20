using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpin : MonoBehaviour
{
    Rigidbody rb;

    //코루틴 대기 시간
    WaitForSeconds ws;

    //코루틴 변수
    private IEnumerator coroutine;

    Vector3 halfSpin = new Vector3(-40, -90);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //회전 시작
    public void SpinStart(int num)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //코루틴 초기화
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = CoSpin(num);

        StartCoroutine(coroutine);
    }

    //코루틴 대기 시간 입력하기
    public void GetSettings(float i)
    {
        ws = new WaitForSeconds(i / 2);
    }

    //주사위 회전 시키기
    IEnumerator CoSpin(int num)
    {
        rb.AddTorque(new Vector3(Random.Range(40,90), Random.Range(180, 360), Random.Range(30, 90)));

        yield return ws;

        rb.AddTorque(halfSpin);

        yield return ws;

        SetDiceResult(num);
    }

    //주사위 회전 함수
    public void SetDiceResult(int result)
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
            //혹시라도 1~6이 아닐 시, 1으로 고정
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
