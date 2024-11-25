using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardCine : MonoBehaviour
{

    void Start()
    {
        CineCamInput();
    }
    public void CineCamInput()
    {
        Billboard[] bill = GameObject.FindObjectsOfType<Billboard>();

        for (int i = 0; i < bill.Length; i++)
        {
            bill[i].SetCamInput(transform.gameObject);
        }
    }
    
    void Update()
    {
        
    }
}
