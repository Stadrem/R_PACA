using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class LeaveRoomMgr : MonoBehaviourPunCallbacks
{
    public void OnClilckLeaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();

        StartCoroutine(WaitForLeaveRoomAndLoadScene());
    }

    private IEnumerator WaitForLeaveRoomAndLoadScene()
    {
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        SceneManager.LoadScene("LobbyScene");
    }
}
