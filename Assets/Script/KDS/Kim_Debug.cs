using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kim_Debug : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            int result = DiceRollManager.Get().DiceRoll(3, 4);
            Debug.Log("반환된 주사위 결과: " + result);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            YarnUiManager.Get().EnableCanvas();
        }
    }
}
