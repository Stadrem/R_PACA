using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class YarnHistoryCard : MonoBehaviour
{
    public Image backgroundColor;
    public TMP_Text text_User;
    public TMP_Text text_Date;
    public TMP_Text text_Time;
    public TMP_Text text_Title;
    public int historyCode = 0;
    public GameObject ui_notFound;
    public GameObject ui_loading;

    string url = "";

    Button button;

    GameObject parent;

    YarnHTTPManager yarnHttpManager;

    private void Start()
    {
        parent = transform.parent.gameObject;

        yarnHttpManager = parent.GetComponent<YarnHTTPManager>();

        button = GetComponent<Button>();
    }

    //웹에서 내용 불러오는 중
    public void Refresh()
    {
        ui_notFound.SetActive(false);
        ui_loading.SetActive(true);
    }

    public void HistroyInfoSetup(string title, string date, string user, string time, int code)
    {
        //네트워크 연결 실패
        if (code == -1)
        {
            text_Title.text = "";
            text_Date.text = "";
            text_Time.text = "";
            text_User.text = "";
            ui_notFound.SetActive(true);
            ui_loading.SetActive(false);
            button.interactable = false;

            //시연 전용
            text_Title.text = "제목: 이세계 모험가";
            text_Date.text = "생성일자: 24.11.14";
            text_Time.text = "플레이타임: 00:05:00";
            text_User.text = "참여자: 유저1, 유저2, 유저3, 유저4";
            historyCode = -2;
            ui_notFound.SetActive(false);
            ui_loading.SetActive(false);
            button.interactable = true;
        }
        //네트워크 연결 성공
        else
        {
            text_Title.text = "제목: " + title;
            text_Date.text = "생성일자: " + date;
            text_Time.text = "플레이타임: " + time;
            text_User.text = "참여자: " + user;
            historyCode = code;
            ui_notFound.SetActive(false);
            ui_loading.SetActive(false);
            button.interactable = true;
        }
    }

    //클릭 시 모험담 내용 팝업
    public void OnClickYarnPopup()
    {
        if(historyCode == -2)
        {
            yarnHttpManager.SimpleTest();
        }
        else
        {
            yarnHttpManager.StartGetYarnContentInfo(historyCode);
        }
    }
}