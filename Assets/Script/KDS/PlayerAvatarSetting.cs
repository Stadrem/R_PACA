using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//Player Prefab 최상단에 붙일 스크립트
public class PlayerAvatarSetting : AvatarHTTPManager
{
    //몸체 3D 게임 오브젝트 저장
    //0번 바디, 1번 헤어, 2번 옷, 3번 무기
    public GameObject[] avatarParts;

    PhotonView pv;

    public MyAvatar myAvatar;

    public TempFakeServer tempFakeServer;

    public bool notUseNetworkOn = false;

    public GameObject ownerCrown;

    public GameObject avatarLoading;

    public GameObject title;

    public TMP_Text titleText;


    private void Awake()
    {
        // 현재 씬의 이름을 가져옴
        //Scene currentScene = SceneManager.GetActiveScene();

        //TempFakeServer가 있다면 뒤에 연결 부분 모두 무시하고 로컬로 실행
        if (GameObject.Find("TempFakeServer") || UserCodeMgr.Instance == null)
        {
            print("백엔드 없는 듯?");
            NotNetwork();
            return;
        }

        //포톤에 연결되었으면, 드래그 회전 기능 비활성화, isMine 기준으로, 클라이언트만 유저코드 받아오기
        if (transform.parent != null && transform.parent.GetComponent<PhotonView>() != null)
        {
            GetComponent<AvatarDragRotate>().enabled = false;

            pv = transform.parent.GetComponent<PhotonView>();

            if (pv.IsMine)
            {
                if(UserCodeMgr.Instance != null)
                {
                    myAvatar.userCode = UserCodeMgr.Instance.UserCode;
                }
            }
        }
        //포톤 연결되어있지 않으면, 포톤이 필요없는 아바타 생성이나 로비이니 드래그 회전 기능 활성화, 일단은 코드 받아옴
        else
        {
            GetComponent<AvatarDragRotate>().enabled = true;

            if (UserCodeMgr.Instance != null)
            {
                myAvatar.userCode = UserCodeMgr.Instance.UserCode;
            }
        }
    }

    void Start()
    {
        RefreshAvatar();

        //SceneManager.sceneLoaded += OnSceneLoaded;


        titleText.text = AchievementManager.Get().GetEquippedAchievement().set.title;


        if (Kim_Debug.instance == null)
        {
            Kim_Debug.Get().DebugPanel();
        }
    }

    /*
    // 씬이 로드될 때마다 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshAvatar();
    }
    */

    //백엔드 없는거 판별되면, 로컬 작동 기초 작업 수행.
    void NotNetwork()
    {
        if(notUseNetworkOn == false && myAvatar.userCode == -1)
        {
            notUseNetworkOn = true;

            myAvatar = TempFakeServer.Get().myAvatar;

            TempFakeServer.Get().FakeSet();

            if (transform.parent != null && transform.parent.GetComponent<PhotonView>() != null)
            {
                pv = transform.parent.GetComponent<PhotonView>();

                if (pv.IsMine)
                {
                    myAvatar.userCode = TempFakeServer.Get().myAvatar.userCode;
                }
            }
        }
    }

    public void RefreshAvatar()
    {
        avatarLoading.SetActive(true);

        //백엔드 없을 시 디버그용
        if (notUseNetworkOn)
        {
            myAvatar = TempFakeServer.Get().myAvatar;

            // 씬 로드 후 실행할 함수 호출
            ChangeAvatar();
        }
        else
        {
            //서버에서 아바타 정보 받아오기
            StartGetAvatarInfo(myAvatar.userCode, (getAvatar) =>
            {
                myAvatar = getAvatar;

                ChangeAvatar();
            });
        }
    }

    /*
    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 등록 해제 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    */

    //0번 게임 오브젝트 : 성별 바디 // 2번: 모자 // 3번: 의류 // 4번: 도구
    public void ChangeAvatar()
    {
        if(myAvatar.userCode == -1)
        {
            NotNetwork();
        }

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
        skinnedMeshRenderer = avatarParts[tempNum].GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.sharedMesh = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].mesh.sharedMesh;
        skinnedMeshRenderer.enabled = true;
        skinnedMeshRenderer.material = AvatarPresetSettings.Get().genderParts[myAvatar.userAvatarGender].avatarParts[tempNum].avatarItems[myAvatar.userAvatarHand].material;

        avatarLoading.SetActive(false);
    }

    //방장 아이콘
    public void ShowOwnerCrown()
    {
        ownerCrown.SetActive(true);
    }
    /*
    private void OnEnable()
    {
        // 이벤트 구독
        if (AchievementManager.Get() != null)
        {
            AchievementManager.Get().OnAchievementChanged += HandleAchievementChangedLocal;
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (AchievementManager.Get() != null)
        {
            AchievementManager.Get().OnAchievementChanged -= HandleAchievementChangedLocal;
        }
    }

    // 업적 변경 감지 시 호출되는 함수
    private void HandleAchievementChangedLocal(string title, int index)
    {
        
    }
    */
}
