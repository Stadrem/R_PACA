using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kim_Debug : MonoBehaviourPunCallbacks
{
    //종합 디버그 시스템. [`] 키 누르면 튀어나옴
    //Get()만 호출해도 알아서 생성됨.
    
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

    //패널 활성화
    public void DebugPanel()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    // [`] 누르면 패널 활성화됨
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugPanel();
        }
    }

    //아바타 정보 주고받을 임시 서버 오브젝트 생성
    public void OnClickNotNetwork()
    {
        TempFakeServer.Get();
    }

    //일상 주사위 굴리기
    public void OnClickDice()
    {
        DiceRollManager.Get().SearchDiceRoll(9);
    }

    //전투 주사위 굴리기
    public void OnClickBattleDice()
    {
        DiceRollManager.Get().BattleDiceRoll(4);
    }

    //엔딩 팝업
    public void OnClickEnding()
    {
        Ending.Get().EnableCanvas();
    }

    //로비로 강제로 돌아가기
    public void OnClickLobby()
    {
        PhotonNetwork.LeaveRoom();

        PhotonNetwork.LoadLevel("LobbyScene");
    }

    //모험담 팝업
    public void OnClickYarnHistroy()
    {
        YarnUiManager.Get().EnableCanvas();
    }

    public int sfxNum = 0;

    public void OnClickSFXTest()
    {
        SoundManager.Get().PlaySFX(sfxNum);
    }

    public int bgmNum = 0;

    public void OnClickBGMTest()
    {
        SoundManager.Get().PlayBGM(bgmNum);
    }

    public void OnClickESC()
    {
        EscUiManager.Get().EnableCanvas();
        print("누름");
    }
}