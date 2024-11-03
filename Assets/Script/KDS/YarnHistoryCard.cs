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

    public void Refresh()
    {
        ui_notFound.SetActive(false);
        ui_loading.SetActive(true);
    }

    public void HistroyInfoSetup(string title, string date, string user, string time, int code)
    {
        if (code == -1)
        {
            text_Title.text = "";
            text_Date.text = "";
            text_Time.text = "";
            text_User.text = "";
            ui_notFound.SetActive(true);
            ui_loading.SetActive(false);
            button.interactable = false;
        }
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

    public void OnClickYarnPopup()
    {
        yarnHttpManager.StartGetYarnContentInfo(historyCode);
    }
}