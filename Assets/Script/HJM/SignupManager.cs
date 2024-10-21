using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignupManager : MonoBehaviour
{
    // InputField로 입력받는 값
    public TMP_InputField userIdField;
    public TMP_InputField passwordField;
    public TMP_InputField nickNameField;

    // 버튼 클릭 시 호출되는 함수
    public void OnClickSignup()
    {
        string username = userIdField.text;
        string password = passwordField.text;
        string nickname = nickNameField.text;

        // 회원가입 요청 보내기
        StartCoroutine(SignupRequest(username, password, nickname));
    }

    // 서버로 회원가입 정보를 POST 요청으로 전송하는 함수
    private IEnumerator SignupRequest(string userId, string password, string nickname)
    {
        // 회원가입 정보를 JSON 형식으로 변환
        SignupData signupData = new SignupData
        {
            userId = userId,
            nickname = nickname,
            password = password
        };
        string json = JsonUtility.ToJson(signupData);

        // 서버에 POST 요청 보내기
        UnityWebRequest request = new UnityWebRequest("http://125.132.216.190:8765/user/register", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청을 보내고 응답 대기
        yield return request.SendWebRequest();

        // 요청 처리 결과 확인
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("회원가입 성공: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("회원가입 실패: " + request.error);
        }
    }

    // 회원가입 데이터
    [System.Serializable]
    public class SignupData
    {
        public string userId;
        public string password;
        public string nickname;
    }
}
