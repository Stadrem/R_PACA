using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnCloseAction : MonoBehaviour
{
    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
