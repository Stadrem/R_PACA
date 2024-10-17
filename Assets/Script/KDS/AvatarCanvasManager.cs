using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerAvatarSetting;

public class AvatarCanvasManager : AvatarHTTPManager
{
    public static AvatarCanvasManager instance;

    public GameObject playerAvatar;

    public GameObject itemsPrefab;

    PlayerAvatarSetting pas;

    public ConnectionMgr cm;

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스 설정
            instance = this;
        }
    }

    //배열 구조체 생성
    public Contents[] contents;

    public void Start()
    {
        //ui 아이콘 생성
        ContentsChildSet(0);

        pas = playerAvatar.GetComponent<PlayerAvatarSetting>();
    }

    //AvatarPresetSettings에 설정한 모든 내용을 기반으로 UI 아이콘 자동 생성
    void ContentsChildSet(int genderNum)
    {
        int genderTemp = genderNum;
        for (int j = 0; j <contents.Length; j++)
        {
            // 기존 자식 게임 오브젝트 삭제
            foreach (Transform child in contents[j].content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            contents[j].children.Clear();

            //Panel A,B,C,D 자식 배열 초기화
            contents[j].children = new List<GameObject>();

            //생성되어있는 요소들 만큼 자식 오브젝트 생성
            for (int i = 0; i < AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems.Length; i++)
            {
                GameObject instance = Instantiate(itemsPrefab);

                instance.transform.SetParent(contents[j].content.transform, false);

                contents[j].children.Add(instance);

                //자식 오브젝트안에 값 할당
                AvatarItemCard itemCard = contents[j].children[i].GetComponent<AvatarItemCard>();

                itemCard.part = j + 1;

                itemCard.itemNum = i;

                itemCard.sprite = AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems[i].sprite;

                //스프라이트 교체
                GameObject temp = itemCard.transform.GetChild(0).gameObject;

                Image originSprite = temp.gameObject.GetComponent<Image>();

                originSprite.sprite = itemCard.sprite;

                //버튼에 액션 할당
                Button button = contents[j].children[i].GetComponent<Button>();

                button.onClick.AddListener(() => PushAvatarCode(itemCard.part, itemCard.itemNum));
            }
        }
    }

    public void OnClickAvatarFinish()
    {
        // POST 요청을 위한 코루틴 실행
        StartPostAvatarInfo(pas.myAvatar);

        TempFakeServer.Get().myAvatar = pas.myAvatar;

        cm.OnClickConnect();
    }

    //public MyAvatar myAvatar;

    public void PushAvatarCode(int parts, int code)
    {
        switch (parts)
        {
            case 0:
                pas.myAvatar.userAvatarGender = code;
                ContentsChildSet(code);
                break;
            case 1:
                pas.myAvatar.userAvatarSkin = code;
                break;
            case 2:
                pas.myAvatar.userAvatarHair = code;
                break;
            case 3:
                pas.myAvatar.userAvatarBody = code;
                break;
            case 4:
                pas.myAvatar.userAvatarHand = code;
                break;
        }

        print("성별 " + pas.myAvatar.userAvatarGender + "/ 부위" + parts + "/ 아이템 넘버 " + code);

        //pas.myAvatar = pas.myAvatar;

        pas.ChangeAvatar();
    }
}

//PanelA/B/C/D 안에 content 값 할당하는 배열 구조체
[System.Serializable]
public struct Contents
{
    public GameObject content; // Content
    public List<GameObject> children; // Child
}