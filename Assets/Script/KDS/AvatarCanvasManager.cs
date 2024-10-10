using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCanvasManager : MonoBehaviour
{
    public static AvatarCanvasManager instance;

    public GameObject itemsPrefab;

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
        //시작 시 아바타 정보 받아오기
        AvatarHTTPManager.instance.StartGetAvatarInfo();

        //ui 아이콘 생성
        ContentsChildSet(0);
    }

    //AvatarPresetSettings에 설정한 모든 내용을 기반으로 UI 아이콘 자동 생성
    void ContentsChildSet(int genderNum)
    {
        int genderTemp = genderNum;
        for (int j = 0; j <contents.Length; j++)
        {
            //Panel A,B,C,D 자식 배열 초기화
            contents[j].children = new List<GameObject>();

            //생성되어있는 요소들 만큼 자식 오브젝트 생성
            for (int i = 0; i < AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems.Length; i++)
            {
                GameObject instance = Instantiate(itemsPrefab);

                instance.transform.SetParent(contents[j].content.transform, false);

                contents[j].children.Add(contents[j].content.transform.GetChild(i).gameObject);

                //자식 오브젝트안에 값 할당
                AvatarItemCard itemCard = contents[j].children[i].GetComponent<AvatarItemCard>();

                itemCard.part = j;

                itemCard.itemNum = i;

                itemCard.itemName = AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems[i].name;
                itemCard.mesh = AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems[i].mesh;
                itemCard.material = AvatarPresetSettings.instance.genderParts[genderTemp].avatarParts[j].avatarItems[i].material;
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
        AvatarHTTPManager.instance.StartPostAvatarInfo();
    }

    public void PushAvatarCode(int parts, int code)
    {
        switch (parts)
        {
            case 0:
                AvatarHTTPManager.instance.myAvatar.userAvatarGender = code;
                ContentsChildSet(code);
                break;
            case 1:
                AvatarHTTPManager.instance.myAvatar.userAvatarSkin = code;
                break;
            case 2:
                AvatarHTTPManager.instance.myAvatar.userAvatarHair = code;
                break;
            case 3:
                AvatarHTTPManager.instance.myAvatar.userAvatarBody = code;
                break;
            case 4:
                AvatarHTTPManager.instance.myAvatar.userAvatarHand = code;
                break;
        }

        print("성별 " + AvatarHTTPManager.instance.myAvatar.userAvatarGender + "/ 부위" + parts + "/ 아이템 넘버 " + code);

        AvatarHTTPManager.instance.AvatarRefresh();
    }
}

//PanelA/B/C/D 안에 content 값 할당하는 배열 구조체
[System.Serializable]
public struct Contents
{
    public GameObject content; // Content
    public List<GameObject> children; // Child
}