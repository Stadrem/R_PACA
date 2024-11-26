using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using System;
using ViewModels;
using Data.Remote.Api;
using UnityEngine.Networking;

public class EndingUiManger : MonoBehaviourPunCallbacks
{
    string url = URLImport.URL;

    public TMP_Text text_title;
    public TMP_Text text_theme;
    public TMP_Text text_user;
    public TMP_Text text_background;
    public TMP_Text text_ch;
    public TMP_Text text_time;
    public TMP_Text text_goalA;
    public TMP_Text text_goalB;

    [System.Serializable]
    public struct ScenarioInfo
    {
        public int roomNum;
        public string roomTitle;
        public string scenarioTitle;
        public string mainQuest;
        public List<string> subQuest;
        public string detail;
        public List<WorldPart> worldParts;
        public List<Player> playerList;
        public List<NPC> npcList;
        public List<string> genre;
        public string startAt;  // DateTime 대신 string 사용
        public string endAt;    // DateTime? 대신 string 사용
    }

    [System.Serializable]
    public struct WorldPart
    {
        // 추가적인 worldParts 정보가 없다면, 필요한 필드를 나중에 정의
    }

    [System.Serializable]
    public struct Player
    {
        public int userCode;
        public string nickname;
        public int userAvatarGender;
        public int userAvatarHair;
        public int userAvatarBody;
        public int userAvatarSkin;
        public int userAvatarHand;
        public int health;
        public int strength;
        public int dex;
        public int? healthPoints;
        public List<string> status;
    }

    [System.Serializable]
    public struct NPC
    {
        public int scenarioAvatarId;
        public string avatarName;
        public int outfit;
        public int health;
        public int strength;
        public int dex;
        public int healthPoints;
        public List<string> status;
    }

    public ScenarioInfo scInfo;

    public void StartPostRoomInfo()
    {
        // HttpInfo 객체 생성
        HttpInfo info = new HttpInfo();

        // 요청할 URL 설정
        info.url = $"{url}/test/running/get?roomNum={PlayUniverseManager.Instance.roomNumber}";

        // 콘텐츠 타입 설정
        info.contentType = "application/json";

        //델리게이트에 그냥 넣기 - 람다식 방식  - 지금 여기선 연산 단계 없음
        info.onComplete = (DownloadHandler downloadHandler) =>
        {
            string json = downloadHandler.text;
            Debug.Log($"Received JSON: {json}");

            try
            {
                scInfo = JsonUtility.FromJson<ScenarioInfo>(json);

                InputPlayerNames();
                InputTitle();
                InputBackground();
                InputCh();
                InputGenre();
                InputTime();
                InputGoal();

                Debug.Log("방 정보 받아오기 성공");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        };

        StartCoroutine(PostRoomInfo(info));
    }

    //정보 가져오기
    public IEnumerator PostRoomInfo(HttpInfo info)
    {
        // GET 요청 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(info.url, "Post"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(info.body);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", info.contentType);

            // 요청 전송 및 응답 대기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 요청 완료 시 처리
                info.onComplete(webRequest.downloadHandler);
            }
            else
            {
                Debug.Log("failed: " + webRequest.error);
            }
        }
    }

    public void OnClickExitLobby()
    {
        AchievementManager.Get().UnlockAchievement(7);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(
                PlayRoomApi.FinishRoom(
                    PlayUniverseManager.Instance.roomNumber,
                    (res) => 
                    {
                        PhotonNetwork.LeaveRoom();
                        PhotonNetwork.LoadLevel("LobbyScene");
                        Destroy(gameObject, 0.5f);
                    }
                )
            );
        }
    }

    void InputPlayerNames()
    {
        text_user.text = "참여자: ";  // 초기화

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            text_user.text += player.NickName + ", ";  // 닉네임 추가
        }
    }

    void InputTitle()
    {
        text_title.text = "제목: " + scInfo.scenarioTitle;
    }

    void InputBackground()
    {
        text_background.text = "등장 배경: ";

        foreach (BackgroundPartInfo background in ViewModelManager.Instance.UniversePlayViewModel.UniverseData.backgroundPartDataList)
        {
            text_background.text += background.Name + ", ";
        }
    }

    void InputGenre()
    {
        text_theme.text = "장르: ";

        foreach (string genre in scInfo.genre)
        {
            if (genre == "Fantasy")
            {
                text_theme.text += "판타지";
            }
            else
            {
                text_theme.text += "판타지";
            }
        }
    }

    void InputTime()
    {
        // 공백 제거
        string startTimeString = scInfo.startAt;

        DateTime startTime;

        try
        {
            // 정확한 형식 지정 및 파싱
            startTime = DateTime.ParseExact(startTimeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            // 현재 시간과의 차이 계산
            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - startTime;

            text_time.text = $"플레이 타임: {timeDifference.Hours}시간 {timeDifference.Minutes}분 {timeDifference.Seconds}초";
        }
        catch (FormatException)
        {
            Debug.LogError("startAt 문자열이 유효한 DateTime 형식이 아닙니다.");
        }
    }

    void InputGoal()
    {
        text_goalA.text = "목표: " + scInfo.mainQuest;
    }

    void InputCh()
    {
        text_ch.text = "등장 캐릭터: ";

        foreach (NPC tempNPC in scInfo.npcList)
        {
            text_ch.text += tempNPC.avatarName + ", ";
        }
    }

    private void Start()
    {
        SoundManager.Get().PlayBGM(5, 0.7f);

        StartPostRoomInfo();
    }

    public void InputText(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
    {
        text_title.text = a;
        text_theme.text = b;
        text_user.text = c;
        text_background.text = d;
        text_ch.text = e;
        text_time.text = h;
        text_goalA.text = i;
        text_goalB.text = j;
    }
}
