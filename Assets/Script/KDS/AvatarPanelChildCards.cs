using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanelChildCards : MonoBehaviour
{
    public int parts;

    //Content 내부에 있는 itemCard 선택 및 값 출력 기능
    GameObject[] childrenPanel;

    // Start is called before the first frame update
    void Start()
    {
        /*
        childrenPanel = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childrenPanel[i] = transform.GetChild(i).gameObject;

            AvatarItemCard num = childrenPanel[i].GetComponent<AvatarItemCard>();

            num.itemNum = i;

            Button button = childrenPanel[i].GetComponent<Button>();

            button.onClick.AddListener(() => AvatarCanvasManager.instance.PushAvatarCode(parts, num.itemNum));
        }
        */
    }
}
