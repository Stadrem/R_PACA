using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Prefab 최상단에 붙일 스크립트
public class PlayerAvatarSetting : MonoBehaviour
{
    //몸체 3D 게임 오브젝트 저장
    public GameObject[] avatarParts;

    //현재 아바타 세팅 저장
    [System.Serializable]
    public struct MyAvatar
    {
        public string userID;
        public int userAvatarGender; //0이면 남자, 1이면 여자.
        public int userAvatarSkin;
        public int userAvatarHair;
        public int userAvatarBody;
        public int userAvatarHand;
    }

    public MyAvatar myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        //자식으로 있는 오브젝트들 받아오기
        avatarParts = new GameObject[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            avatarParts[i] = transform.GetChild(i).gameObject;
        }

        ChangeAvatar();
    }

    //0번 게임 오브젝트 : 성별 바디 // 2번: 모자 // 3번: 의류 // 4번: 도구
    public void ChangeAvatar()
    {
        int tempNum = 0;
        //성별
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].mesh.sharedMesh;
        //피부
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarSkin].material;

        //모자
        tempNum = 1;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].mesh.sharedMesh;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].material;

        //의류
        tempNum = 2;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].mesh.sharedMesh;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].material;

        //손
        tempNum = 3;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].mesh.sharedMesh;
        avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>().material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].material;
    }
}
