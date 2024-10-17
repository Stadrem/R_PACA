using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Prefab 최상단에 붙일 스크립트
public class PlayerAvatarSetting : MonoBehaviour
{
    //몸체 3D 게임 오브젝트 저장
    //0번 바디, 1번 헤어, 2번 옷, 3번 무기
    public GameObject[] avatarParts;

    PhotonView pv;

    UserInfo info;

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

    private void Awake()
    {
        pv = transform.parent.GetComponent<PhotonView>();

        info = transform.parent.GetComponent<UserInfo>();

        AvatarHTTPManager.Get().StartGetAvatarInfo(info.userID);
    }

    void Start()
    {
        /*
        //자식으로 있는 오브젝트들 받아오기
        avatarParts = new GameObject[transform.childCount];


        for(int i = 0; i < transform.childCount; i++)
        {
            avatarParts[i] = transform.GetChild(i).gameObject;
        }
        */

        ChangeAvatar();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeAvatar();
        }
    }

    //0번 게임 오브젝트 : 성별 바디 // 2번: 모자 // 3번: 의류 // 4번: 도구
    public void ChangeAvatar()
    {
        //이 알 수 없는 로직에 대한 설명
        //스킨메쉬 렌더러 컴포넌트 받아오기 -> 스킨메쉬 컴포넌트 비활성 -> 비우기 -> 받아오기 -> 컴포넌트 활성화
        //이따위로 해놓은 이유 : 스키닝 데이터와 버텍스 데이터가 틀리다고 에러 계속 뿜어대서, 해결 방안이 이것이었음

        //성별
        int tempNum = 0;
        SkinnedMeshRenderer skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        //Transform[] originalBones = skinnedMeshRenderer.bones;
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].mesh.sharedMesh;
        //skinnedMeshRenderer.bones = originalBones;
        skinnedMeshRenderer.enabled = true;
        //피부
        skinnedMeshRenderer.material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarSkin].material;

        //모자
        tempNum = 1;
        skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].mesh.sharedMesh;
        //skinnedMeshRenderer.bones = originalBones;
        skinnedMeshRenderer.enabled = true;
        skinnedMeshRenderer.material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].material;

        //의류
        tempNum = 2;
        skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].mesh.sharedMesh;
        //skinnedMeshRenderer.bones = originalBones;
        skinnedMeshRenderer.enabled = true;
        skinnedMeshRenderer.material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].material;

        //손
        tempNum = 3;

        MeshFilter meshFilter = avatarParts[tempNum].GetComponent<MeshFilter>();
        meshFilter.mesh = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].meshB.sharedMesh;
        MeshRenderer meshRenderer = avatarParts[tempNum].GetComponent<MeshRenderer>();
        meshRenderer.material = AvatarPresetSettings.instance.genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].material;
    }
}
