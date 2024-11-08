using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotonChatMgr : MonoBehaviour, IChatClientListener
{
    // 채팅을 총괄하는 객체
    ChatClient chatClient;

    // 채팅 입력 UI
    public TMP_InputField inputChat;

    // 채팅 채널 이름
    string currChannel = "채팅";

    // 스크롤 뷰의 Content
    public RectTransform trContent;
    // ChatItem Prefab
    public GameObject chatItemFactory;

    void Start()
    {
        // 채팅내용을 작성하고 엔터를 쳤을때 호출되는 함수 등록
        inputChat.onSubmit.AddListener(OnSubmit);

        Connect();
    }

    void Update()
    {
        // 채팅서버에서 오는 응답을 수신하기 위해서 계속 호출 해줘야 한다.        
        if(chatClient != null)
        {
            chatClient.Service();

        }

        if(Input.GetKeyDown(KeyCode.F5))
        {
            // 채널에서 나가자
            string[] channels = { currChannel };
            chatClient.Unsubscribe(channels);
        }
    }

    void Connect()
    {

        // 포톤 설정을 가져오자
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        // 위 설정을 가지고 ChatAppSettings 셋팅
        ChatAppSettings chatAppSettings = new ChatAppSettings();
        chatAppSettings.AppIdChat = photonSettings.AppIdChat;
        chatAppSettings.AppVersion = photonSettings.AppVersion;
        chatAppSettings.FixedRegion = photonSettings.FixedRegion;
        chatAppSettings.NetworkLogging = photonSettings.NetworkLogging;
        chatAppSettings.Protocol = photonSettings.Protocol;
        chatAppSettings.EnableProtocolFallback = photonSettings.EnableProtocolFallback;
        chatAppSettings.Server = photonSettings.Server;
        chatAppSettings.Port = (ushort)photonSettings.Port;
        chatAppSettings.ProxyServer = photonSettings.ProxyServer;

        // ChatClinet 만들자.
        chatClient = new ChatClient(this);


        // 닉네임
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName);
        // 연결 시도
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    void OnSubmit(string s)
    {
        // 만약에 s 의 길이가 0이면 함수를 나가자.
        if (s.Length == 0) return;

        // 귓속말인 판단
        // /w 아이디 메시지 (/w 닉네임 안녕하세요 반갑습니다.)
        string [] splitChat = s.Split(" ", 3);
        
        if (splitChat[0] == "/w")
        {
            // 귓속말을 보내자
            // splitchat[1] : 아이디 , splitChat[2] : 내용
            chatClient.SendPrivateMessage(splitChat[1], splitChat[2]);
        }
        else
        {
            print("11111111");

            // 채팅을 보내자.
            chatClient.PublishMessage(currChannel, s);
        }

        // 채팅 입력란 초기화
        inputChat.text = "";
    }

    void CreateChatItem(string sender, object message, Color messageColor)
    {
        // ChatItem 생성 (Content 의 자식으로)
        GameObject go = Instantiate(chatItemFactory, trContent);
        // 생성된 게임오브젝트에서 ChatItem 컴포넌트 가져온다.
        ChatItem chatItem = go.GetComponent<ChatItem>();

        // 닉네임을 노란색으로 설정
        string formattedMessage = $"<color=yellow>{sender}</color> : {message}";

        // 가져온 컴포넌트에서 SetText 함수 실행
        chatItem.SetText(formattedMessage);
    }


    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
    }

    // 채팅 서버에 접속이 성공하면 호출되는 함수
    public void OnConnected()
    {
        print("채팅 서버 접속 성공!");
        // 전체 채널에 들어가자( 구독 )
        chatClient.Subscribe(currChannel);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    // 특정 채널에 다른 사람(나)이 메시지를 보내고 나한테 응답이 올때 호출 되는 함수
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            print(senders[i] + " : " + messages[i]);

            CreateChatItem(senders[i], messages[i], Color.white);
        }        
    }

    // 누군가 나한테 개인메시지를 보냈을 때
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CreateChatItem(sender, message, Color.blue);
    }

    // 채팅 채널에 접속이 성공했을 때 들어오는 함수
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i < channels.Length; i++)
        {
            print(channels[i] + " 채널에 접속이 성공 했습니다" );
        }
    }

    // 채팅 채널에서 나갔을 때 들어오는 함수
    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            print(channels[i] + " 채널에서 나갔습니다");
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}
