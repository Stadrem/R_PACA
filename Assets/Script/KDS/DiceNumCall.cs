using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceNumCall : MonoBehaviour
{
    public GameObject[] coll;

    Rigidbody rb;

    public bool checkOut = false;

    public System.Action<int> onDiceResult; // 주사위 결과 콜백

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < 2 && rb.velocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.01f && checkOut == false)
        {
            checkOut = true;

            OnTriggerOn();
        }
    }

    public void OnTriggerOn()
    {
        for (int i = 0; i < coll.Length; i++)
        {
            coll[i].SetActive(true);
        }
    }

    public void OnDiceStopped(int result)
    {
        onDiceResult?.Invoke(result); // 주사위 결과를 콜백으로 전달

        for (int i = 0; i < coll.Length; i++)
        {
            coll[i].SetActive(false);
        }
    }
}
