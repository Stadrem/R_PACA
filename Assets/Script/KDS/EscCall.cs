using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscCall : MonoBehaviour
{
    public void OnClickEsc()
    {
        EscUiManager.Get().EnableCanvas();
    }
}
