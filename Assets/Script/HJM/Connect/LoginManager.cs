using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField userIdField;
    public TMP_InputField passwordField;

    private ConnectionMgr connection;

    private void Start()
    {
        // ConnectionMgr 컴포넌트를 찾습니다.
        connection = GetComponent<ConnectionMgr>();
    }

    public void OnClickLogin()
    {
        string userId = userIdField.text;
        string password = passwordField.text;

        // 필드 확인
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("아이디와 비밀번호를 모두 입력해야 합니다.");
            return;
        }

        Debug.Log("UserID 입력값: " + userId);
        StartCoroutine(LoginRequest(userId, password));
        Debug.Log("로그인 시도: " + userId);
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

                // UserCodeMgr에 userCode 저장
                UserCodeMgr.Instance.SetUserCode(response.userCode); // userCode 저장

                // 연결이 되어 있다면 마스터 서버에 연결
                if (PhotonNetwork.IsConnected)
                {
                    Debug.Log("이미 Photon 서버에 연결되어 있습니다.");
                }
                else
                {
                    connection.OnClickConnect(); // 서버에 연결
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
        public int userCode; // 서버의 응답에서 userCode를 저장
    }
}
