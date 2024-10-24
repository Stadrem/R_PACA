using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userIdField;
    public TMP_InputField passwordField;

    public ConnectionMgr connection;
    public void OnClickLogin()
    {
        string userId = userIdField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("아이디와 비밀번호를 모두 입력해야 합니다.");
            return;
        }

        Debug.Log("UserID 입력값: " + userId);
        connection = GetComponent<ConnectionMgr>();
        StartCoroutine(LoginRequest(userId, password));
        Debug.Log("로그인 시도" + userId);
    }

    private IEnumerator LoginRequest(string userId, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post("http://125.132.216.190:8765/login", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("로그인 성공: " + request.downloadHandler.text);

                // 서버에서 받은 JSON 응답을 LoginResponse 클래스로 변환
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // UserManager 싱글톤에 userCode 저장
                UserCodeMgr.Instance.SetUserCode(response.userCode); // int형으로 저장

                // ConnectionMgr가 null이 아니라면
                if (connection != null)
                {
                    connection.OnClickConnect(); // 마스터 서버에 연결
                }
            }
            else
            {
                Debug.LogError("로그인 실패: " + request.error);
                Debug.LogError("응답 본문: " + request.downloadHandler.text);
            }
        }
    }

    // 서버 응답을 처리하기 위한 클래스
    [System.Serializable]
    public class LoginResponse
    {
        // 서버의 응답에서 userCode를 저장
        public int userCode;
    }
}
