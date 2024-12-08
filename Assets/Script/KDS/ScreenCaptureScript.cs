using UnityEngine;

public class ScreenCaptureScript : MonoBehaviour
{
    // 캡처된 파일 저장 위치
    private string screenshotPath;

    public Animator anim;

    void Start()
    {
        // 저장 경로 설정 (프로젝트 폴더 내 "Screenshots" 폴더)
        screenshotPath = Application.dataPath + "/Screenshots/";

        // 폴더가 없으면 생성
        if (!System.IO.Directory.Exists(screenshotPath))
        {
            System.IO.Directory.CreateDirectory(screenshotPath);
        }
    }

    void Update()
    {
        // Z 키를 누르면 화면 캡처
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CaptureScreenshot();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PoseChange();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("End");
        }
    }

    void CaptureScreenshot()
    {
        // 파일 이름 생성 (타임스탬프)
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        // 파일 전체 경로
        string fullPath = screenshotPath + fileName;

        // 스크린샷 캡처
        ScreenCapture.CaptureScreenshot(fullPath);

        Debug.Log("스크린샷 저장됨: " + fullPath);
    }

    int poseNum = 0;

    void PoseChange()
    {
        if (poseNum == 4)
        {
            poseNum = 0;
        }

        switch (poseNum)
        {
            case 0:
                anim.SetTrigger("Action1");
                poseNum++;
                break;
            case 1:
                anim.SetTrigger("Action2");
                poseNum++;
                break;
            case 2:
                anim.SetTrigger("Action3");
                poseNum++;
                break;
            case 3:
                anim.SetTrigger("Action4");
                poseNum++;
                break;
        }
    }
}