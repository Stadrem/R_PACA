using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStats : MonoBehaviour
{
    public static UserStats Instance { get; private set; }

    [Header("유저 스탯")]
    public int userHealth; // 생명력
    public int userStrength; // 힘
    public int userDexterity; // 손재주

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 유저 정보 초기화
    public void Initialize(

        int health,
        int strength,
        int dexterity)
    {

        userHealth = health;
        userStrength = strength;
        userDexterity = dexterity;
    }

}
