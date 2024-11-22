using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonChatMgr : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    public TMP_InputField inputChat;
    string currChannel = "";
    public RectTransform trContent;
    public GameObject chatItemFactory;


    private bool hasChangedChannel = false;

    void Start()
    {
        inputChat.onSubmit.AddListener(OnSubmit);
        Connect();


        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            string[] channels = { currChannel };
            chatClient.Unsubscribe(channels);
        }
    }

    void Connect()
    {
        AppSettings photonSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
        ChatAppSettings chatAppSettings = new ChatAppSettings
        {
            AppIdChat = photonSettings.AppIdChat,
            AppVersion = photonSettings.AppVersion,
            FixedRegion = photonSettings.FixedRegion,
            NetworkLogging = photonSettings.NetworkLogging,
            Protocol = photonSettings.Protocol,
            EnableProtocolFallback = photonSettings.EnableProtocolFallback,
            Server = photonSettings.Server,
            Port = (ushort)photonSettings.Port,
            ProxyServer = photonSettings.ProxyServer
        };

        chatClient = new ChatClient(this);
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName);
        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    void OnSubmit(string s)
    {
        if (s.Length == 0) return;

        string[] splitChat = s.Split(" ", 3);
        if (splitChat[0] == "/w")
        {
            chatClient.SendPrivateMessage(splitChat[1], splitChat[2]);
        }
        else
        {
            chatClient.PublishMessage(currChannel, s);
        }
        inputChat.text = "";
    }

    void CreateChatItem(string sender, object message, Color messageColor)
    {
        GameObject go = Instantiate(chatItemFactory, trContent);
        ChatItem chatItem = go.GetComponent<ChatItem>();
        string formattedMessage = $"<color=yellow>{sender}</color> : {message}";
        chatItem.SetText(formattedMessage);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (!hasChangedChannel)
        {
            // 기존채팅 아이템 삭제
            foreach (Transform child in trContent)
            {
                Destroy(child.gameObject);
            }

            // 이전 채널 해제
            if (!string.IsNullOrEmpty(currChannel))
            {
                chatClient.Unsubscribe(new string[] { currChannel });

            }

            // 새로운 채널 구독
            string newChannel = currChannel + "_inGame";
            chatClient.Subscribe(new string[] { newChannel });
            Debug.Log("새로운" + newChannel + "채널을 구독합니다");



            currChannel = newChannel;
            hasChangedChannel = true;
        }
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Debug Level: {level} - Message: {message}");
    }

    public void OnDisconnected()
    {

    }

    public void OnConnected()
    {
        currChannel = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(currChannel);
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            Debug.Log($"{senders[i]}: {messages[i]}");
            CreateChatItem(senders[i], messages[i], Color.white);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

        CreateChatItem(sender, message, Color.blue);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log($"{channels[i]} 채널에 접속");
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log($"{channels[i]} 채널에서 나감");
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
