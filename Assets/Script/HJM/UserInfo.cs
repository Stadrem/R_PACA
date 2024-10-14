using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    [Header("유저 회원정보")]
    
    // 로그인 시 유저 ID, PW, 닉네임
    public string userID;
    public string userPassword;
    public string userName;

    [Header("아바타 정보")]
    public int userAvatarGender; // 성별에 따라 Body, Hair는 통채로 바뀜
    public int userAvatarHair;
    public int userAvatarBody;
    public int userAvatarSkin;
    public int userAvatarHand;


    // 유저 정보 초기화
    public void Initialize(string inputID, string inputPW, string inputName, int gender, int hair, int body, int skin, int hand)
    {
        userID = inputID;
        userPassword = inputPW;
        userName = inputName;

        userAvatarGender = gender;
        userAvatarHair = hair;
        userAvatarBody = body;
        userAvatarSkin = skin;
        userAvatarHand = hand;
    }
}