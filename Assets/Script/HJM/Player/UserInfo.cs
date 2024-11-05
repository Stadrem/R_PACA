using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    [Header("유저 회원정보")]
    // 로그인 시 유저 ID, 닉네임
    public string userID;
    public string userName;

    [Header("유저 스탯")]
    public int userHealth; // 생명력
    public int userStrength; // 힘
    public int userDexterity; // 손재주

    // 유저 정보 초기화
    public void Initialize(
        string inputID,
        string inputName,
        int health,
        int strength,
        int dexterity)
    {
        userID = inputID;
        userName = inputName;

        // 유저 스탯 초기화
        userHealth = health;
        userStrength = strength;
        userDexterity = dexterity;
    }
}
