using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardInput : MonoBehaviour
{
    public void CamInput()
    {
        Billboard[] bill = GameObject.FindObjectsOfType<Billboard>();

        for (int i = 0; i < bill.Length; i++)
        {
            bill[i].cam = GetComponent<Camera>();
        }
    }
}
