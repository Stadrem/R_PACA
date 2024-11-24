using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class GameMasterServerEventManager : MonoBehaviour
    {
        private readonly string sseURL = $"{HttpManager.ServerURL}/gm";

        public Action<string> OnEventReceived;


        private static GameMasterServerEventManager instance;
        private int currentRoomId;
        private Coroutine currentEventCoroutine;

        public static GameMasterServerEventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("GM_ServerEvent_Manager").AddComponent<GameMasterServerEventManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Disconnect()
        {
            StartCoroutine(CoDisconnect());
        }
        
        private IEnumerator CoDisconnect()
        {
            var url = $"{sseURL}/disconnect?roomId={currentRoomId}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Accept", "text/event-stream");

                // SSE 요청 전송
                yield return request.SendWebRequest();
            }
        }


        public void Connect(int roomId)
        {
            currentRoomId = roomId;
            currentEventCoroutine = StartCoroutine(CoConnect(roomId));
        }

        private IEnumerator CoConnect(int roomId)
        {
            Debug.Log($"Connect to SSE: {roomId}");
            var url = $"{sseURL}/connect?roomId={roomId}";
            var downloadHandler = new ClearableDownloadHandler();
            ;
            using (UnityWebRequest request = new UnityWebRequest(
                       url,
                       "GET",
                       (DownloadHandler)downloadHandler,
                       (UploadHandler)null
                   ))
            {
                request.SetRequestHeader("Accept", "text/event-stream");

                // SSE 요청 전송
                request.SendWebRequest();

                // 연결 유지
                while (!request.isDone)
                {
                    if (request.result == UnityWebRequest.Result.ConnectionError
                        || request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError($"SSE Error: {request.error}");
                        yield break;
                    }

                    new WaitUntil(() => downloadHandler.isDone);

                    string responseText = downloadHandler.GetData();
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        OnEventReceived?.Invoke(responseText);
                        Debug.Log($"SSE Data Received: {responseText}");
                        downloadHandler.ClearData();
                    }


                    yield return null; // 잠시 대기 후 반복
                }
            }
        }
    }
}