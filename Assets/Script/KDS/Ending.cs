using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviourPunCallbacks
{
    //싱글톤
    public static Ending instance;

    public static Ending Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Canvas_Ending");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<Ending>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
    }

    EndingUiManger eum;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        eum = GetComponent<EndingUiManger>();
    }

    public void EnableCanvas()
    {
        gameObject.SetActive(true);
    }

    public void EnableCanvas(bool win)
    {
        gameObject.SetActive(true);

        if (win)
        {
            eum.text_goalB.text = "달성!";
        }
        else
        {
            eum.text_goalB.text = "실패!";
        }
    }
}
