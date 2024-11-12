using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static HttpManager;

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
            Alert.Get().Set("아이디와 비밀번호를 모두 입력해야 합니다.");

            Debug.LogError("아이디와 비밀번호를 모두 입력해야 합니다.");
            return;
        }

        Debug.Log("UserID 입력값: " + userId);
        StartCoroutine(LoginRequest(userId, password));
        Debug.Log("로그인 시도: " + userId);

        Alert.Get().Set("로그인 시도 중...");
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
                Alert.Get().Set("로그인 성공");
                Debug.Log("로그인 성공: " + request.downloadHandler.text);

                // 서버에서 받은 JSON 응답을 LoginResponse 클래스로 변환
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // userCode 저장
                UserCodeMgr.Instance.SetUserCode(response.userCode);



                // 추가 정보 요청
                StartCoroutine(GetUserInfo(response.userCode));

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

    private IEnumerator GetUserInfo(int userCode)
    {
        string url = "http://125.132.216.190:8765/user/detail?userCode=" + userCode;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("유저 정보 가져오기 성공: " + request.downloadHandler.text);

                // 서버에서 받은 JSON 응답을 UserInfoResponse 클래스로 변환
                UserInfoResponse response = JsonUtility.FromJson<UserInfoResponse>(request.downloadHandler.text);

                // UserCodeMgr에 userID와 nickname 저장
                UserCodeMgr.Instance.SetUserInfo(response.userId, response.nickname);
                
                var properties = new ExitGames.Client.Photon.Hashtable
                {
                    { PunPropertyNames.Player.PlayerId, UserCodeMgr.Instance.UserID },
                    { PunPropertyNames.Player.PlayerUserCode, UserCodeMgr.Instance.UserCode },
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
                // Photon의 NickName을 갱신
                PhotonNetwork.NickName = UserCodeMgr.Instance.Nickname;
            }
            else
            {
                Debug.LogError("유저 정보 가져오기 실패: " + request.error);
            }
        }
    }

    // 로그인 응답을 처리하기 위한 클래스
    [System.Serializable]
    public class LoginResponse
    {
        public int userCode; // 서버의 응답에서 userCode를 저장
    }

    // 유저 정보 응답을 처리하기 위한 클래스
    [System.Serializable]
    public class UserInfoResponse
    {
        public string userId; // 유저의 ID
        public string nickname; // 유저의 닉네임
    }
}