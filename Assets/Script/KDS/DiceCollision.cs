using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollision : MonoBehaviour
{
    public int diceNumber;

    public DiceNumCall diceNumCall;

    private void Start()
    {
        diceNumCall = GetComponentInParent<DiceNumCall>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            diceNumCall.OnDiceStopped(diceNumber); // 주사위 번호 전달
        }
        gameObject.SetActive(false);
    }
}