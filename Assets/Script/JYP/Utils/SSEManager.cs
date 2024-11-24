using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class SSEManager : MonoBehaviour
    {
        private readonly string sseURL = $"http://125.132.216.190:9876/gm";

        public Action<string> OnSSEDataReceived;
        

        private static SSEManager instance;

        public static SSEManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("SSEManager").AddComponent<SSEManager>();
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


        private IEnumerator DisconnectToSSE()
        {
            var url = $"{sseURL}/disconnect?roomId={roomId}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                    // 응답 스트림에서 데이터 읽기
                    while (request.downloadHandler.isDone == false)
                    {
                        string responseText = request.downloadHandler.text;
                        if (!string.IsNullOrEmpty(responseText))
                        {
                            Debug.Log($"SSE Data Received: {responseText}");
                        }

                        yield return null; // 잠시 대기 후 반복
                    }
                }
            }
        }

        private int roomId;

        public IEnumerator ConnectToSSE(int roomId)
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
                        OnSSEDataReceived?.Invoke(responseText);
                        Debug.Log($"SSE Data Received: {responseText}");
                        downloadHandler.ClearData();
                    }


                    yield return null; // 잠시 대기 후 반복
                }
            }
        }
    }
}