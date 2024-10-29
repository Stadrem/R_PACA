using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderData;

public class Kim_Debug : MonoBehaviour
{
    //싱글톤
    public static Kim_Debug instance;

    public static Kim_Debug Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/Ui/Kim_Debug");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<Kim_Debug>();

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject debugPanel;

    //백엔드 없을 시 디버그용
    void DebugPanel()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugPanel();
        }
    }

    public void OnClickNotNetwork()
    {
        TempFakeServer.Get();
    }

    public void OnClickDice()
    {
        DiceRollManager.Get().DiceRoll(3, 4, true);
    }

    public void OnClickEnding()
    {
        Ending.Get().EnableCanvas();
    }
}

