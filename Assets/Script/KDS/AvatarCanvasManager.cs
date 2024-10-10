using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCanvasManager : MonoBehaviour
{
    public static AvatarCanvasManager instance;

    public AvatarPresetSettings aps;

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스 설정
            instance = this;
        }
    }

    public Contents[] contents; // 여러 Content 구조체 배열

    public void Start()
    {
        AvatarHTTPManager.instance.StartGetAvatarInfo();
        ContentsChildSet();
    }

    void ContentsChildSet()
    {
        for(int j = 0; j <contents.Length; j++)
        {
            contents[j].children = new List<GameObject>();

            for (int i = 0; i < contents[j].content.transform.childCount; i++)
            {
                contents[j].children.Add(contents[j].content.transform.GetChild(i).gameObject);

                AvatarItemCard num = contents[j].children[i].GetComponent<AvatarItemCard>();

                num.part = j;

                num.itemNum = i;

                Button button = contents[j].children[i].GetComponent<Button>();

                button.onClick.AddListener(() => PushAvatarCode(num.part, num.itemNum));
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

        print("부위" + parts + "/ 아이템 넘버 " + code);

        AvatarHTTPManager.instance.AvatarRefresh();
    }
}

[System.Serializable]
public struct Contents
{
    public GameObject content; // Content
    public List<GameObject> children; // Child
}