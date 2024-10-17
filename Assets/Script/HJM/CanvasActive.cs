using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasActive : MonoBehaviour
{
    public GameObject canvasPop;
    public GameObject canvasExit;

    public void OnClickPop()
    {
        if (canvasPop == null) return;
        canvasPop.SetActive(true);
    }

    public void OnClickExit()
    {
        if (canvasExit == null) return;
        canvasExit.SetActive(false);
    }
}
