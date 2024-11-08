using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasActive : MonoBehaviour
{
    public GameObject canvasPop;
    public GameObject canvasExit;
    public GameObject canvasDestroyed;


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

    public void OnClickDestroyed()
    {
        if (canvasDestroyed == null) return;
        Destroy(canvasDestroyed);
    }

    public void OnClickHistory()
    {
        YarnUiManager.Get().EnableCanvas();
    }

    public void OnClickAvatar()
    {
        SceneManager.LoadScene("AvatarCreate");
    }

    public void OnClickGameScene()
    {
        PhotonNetwork.LoadLevel("Town");
    }
}