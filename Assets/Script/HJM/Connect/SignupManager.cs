using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SignupManager : MonoBehaviour
{
    // InputField로 입력받는 값
    public TMP_InputField userIdField;
    public TMP_InputField passwordField;
    public TMP_InputField nickNameField;

    // 회원가입 성공 안내 UI
    public GameObject signupSuccessUI;

    private void Start()
    {
    }

    // 회원가입 버튼 클릭 시 호출되는 함수
    public void OnClickSignup()
    {
        string userId = userIdField.text;
        string password = passwordField.text;
        string nickname = nickNameField.text;

        // 모든 필드가 입력되었는지 확인
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            Debug.LogError("모든 필드를 입력해야 합니다.");
            return; // 필드가 비어있을 경우 종료
        }

        Debug.Log("UserID 입력값: " + userId); // 확인용 로그

        // 회원가입 요청 보내기
        StartCoroutine(SignupRequest(userId, password, nickname));
    }

    // 서버로 회원가입 정보를 POST 요청으로 전송하는 함수
    private IEnumerator SignupRequest(string userId, string password, string nickname)
    {
        // MultipartFormData를 사용하여 폼 데이터와 파일 업로드 준비
        WWWForm form = new WWWForm();
        form.AddField("userId", userId); // 사용자 ID 필드 추가
        form.AddField("password", password); // 비밀번호 필드 추가
        form.AddField("nickname", nickname); // 닉네임 필드 추가

        // 서버에 POST 요청 보내기
        using (UnityWebRequest request = UnityWebRequest.Post("http://125.132.216.190:8765/user/register", form))
        {
            // 요청을 보내고 응답 대기
            yield return request.SendWebRequest();

            // 요청 처리 결과 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원가입 성공: " + request.downloadHandler.text);

                // 회원가입 성공 UI 활성화
                signupSuccessUI.SetActive(true);

                // 1초 대기 후 로그인 시도
                yield return new WaitForSeconds(1f);

                // 동일한 ID와 PW로 로그인 시도
                StartCoroutine(LoginRequest(userId, password));
            }
            else
            {
                Debug.LogError("회원가입 실패: " + request.error);
                Debug.LogError("응답 본문: " + request.downloadHandler.text); // 추가 디버깅
            }
        }
    }

    // 서버로 로그인 요청을 보내는 함수
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

                // 1초 대기 후 AvatarCreate 씬으로 이동
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("AvatarCreate"); // 아바타 생성 씬으로 이동
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
