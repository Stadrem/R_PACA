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
            DiceRollManager.Get().DiceRoll(3, 4, true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            YarnUiManager.Get().EnableCanvas();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bool result = DiceRollManager.Get().BattleDiceRoll(0);
            Debug.Log("반환된 주사위 결과: " + result);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Ending.Get().EnableCanvas();
            print("엔딩 발동");
        }
    }
}
