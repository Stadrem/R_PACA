using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Player Prefab 최상단에 붙일 스크립트
public class PlayerAvatarSetting : AvatarHTTPManager
{
    //몸체 3D 게임 오브젝트 저장
    //0번 바디, 1번 헤어, 2번 옷, 3번 무기
    public GameObject[] avatarParts;

    PhotonView pv;

    //GetMyAvatar info;

    public MyAvatar myAvatar;

    TempFakeServer tfs;

    bool notUseNetworkOn = false;

    private void Awake()
    {
        // 현재 씬의 이름을 가져옴
        Scene currentScene = SceneManager.GetActiveScene();

        // 씬 이름 비교
        if (currentScene.name != "AvatarCreate")
        {
            pv = transform.parent.GetComponent<PhotonView>();
        }
    }

    void Start()
    {
        RefreshAvatar();

        SceneManager.sceneLoaded += OnSceneLoaded;


        //백엔드 없을 시 디버그 전용. 제거해도 됨.
        if (GameObject.Find("TempFakeServer"))
        {
            tfs = GameObject.Find("TempFakeServer").GetComponent<TempFakeServer>();

            notUseNetworkOn = true;

            print("백엔드 없음");
        }
        else
        {
            print("정상 접속 상황");
        }
    }

    // 씬이 로드될 때마다 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshAvatar();
    }

    void RefreshAvatar()
    {
        //myAvatar.userCode = TempFakeServer.Get().myAvatar.userCode;

        //서버에서 아바타 정보 받아오기
        StartGetAvatarInfo(myAvatar.userCode, (getAvatar) =>
        {
            myAvatar = getAvatar;

            ChangeAvatar();
        });

        //백엔드 없을 시 디버그용
        if (notUseNetworkOn)
        {
            myAvatar = TempFakeServer.Get().myAvatar;
        }

        // 씬 로드 후 실행할 함수 호출
        ChangeAvatar();
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 등록 해제 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //0번 게임 오브젝트 : 성별 바디 // 2번: 모자 // 3번: 의류 // 4번: 도구
    public void ChangeAvatar()
    {
        //이 알 수 없는 로직에 대한 설명
        //스킨메쉬 렌더러 컴포넌트 받아오기 -> 스킨메쉬 컴포넌트 비활성 -> 비우기 -> 받아오기 -> 컴포넌트 활성화
        //이따위로 해놓은 이유 : 교체 전 스키닝 메쉬 데이터와 교체 후 메쉬 데이터의 버텍스 값이 틀리다고 에러 계속 뿜어대서, 해결 방안이 이것이었음

        //성별
        int tempNum = 0;
        SkinnedMeshRenderer skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        //skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].mesh.sharedMesh;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[2].avatarItems[myAvatar.userAvatarBody].subMesh.sharedMesh;
        skinnedMeshRenderer.enabled = true;
        //피부
        skinnedMeshRenderer.material = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarSkin].material;

        //모자
        tempNum = 1;
        skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].mesh.sharedMesh;
        skinnedMeshRenderer.enabled = true;
        skinnedMeshRenderer.material = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHair].material;

        //의류
        tempNum = 2;
        skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].mesh.sharedMesh;
        skinnedMeshRenderer.enabled = true;
        skinnedMeshRenderer.material = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarBody].material;

        //손
        tempNum = 3;
        MeshFilter meshFilter = avatarParts[tempNum].GetComponent<MeshFilter>();
        meshFilter.mesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].meshB.sharedMesh;
        MeshRenderer meshRenderer = avatarParts[tempNum].GetComponent<MeshRenderer>();
        meshRenderer.material = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].material;
    }
}
