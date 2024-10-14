using System;
using UnityEngine;

public class UniversePlayManager : MonoBehaviour
{
    private static UniversePlayManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
}