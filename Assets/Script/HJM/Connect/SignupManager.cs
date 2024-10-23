using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SignupManager : MonoBehaviour
{
    // InputField로 입력받는 값
    public TMP_InputField userIdField;
    public TMP_InputField passwordField;
    public TMP_InputField nickNameField;

    // 버튼 클릭 시 호출되는 함수
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
            }
            else
            {
                Debug.LogError("회원가입 실패: " + request.error);
                Debug.LogError("응답 본문: " + request.downloadHandler.text); // 추가 디버깅
            }
        }
    }
}
