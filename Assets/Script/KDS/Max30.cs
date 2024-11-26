using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Max30 : MonoBehaviour
{
    //싱글톤
    public static Max30 instance;

    public static Max30 Get()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool maxOver = false;

    public bool notInput = true;
}
