using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DummyCSV : MonoBehaviour
{
    static DummyCSV instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Parse("uniDummyData");
    }
    public void Parse(string fileName) // 혹은 path
    {
        // flieName에 해당되는 file을 읽어오자
        string path = Application.dataPath + "/Script/HJM/TestData/" + fileName + ".csv";
        print(path);

        string strData = File.ReadAllText(path);
        print(strData);

        // 엔터를 기준으로 한 줄씩 자르자

        // "\n" 을 기준으로 자르자.
        string [] lines = strData.Split("\n");
        // "\r" 을 기준으로 자르자.
        for (int i = 0; i < lines.Length; i++)
        {
            string [] temp = lines[i].Split("\r");
            lines[i] = temp[0];
            print(lines[i]);
        }
    }
}
