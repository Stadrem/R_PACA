using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class SSEManager : MonoBehaviour
    {
        private readonly string sseURL = $"{HttpManager.ServerURL}/sse";

        public string ReceivedData { get; private set; }


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


        private void Start()
        {
            StartCoroutine(ConnectToSSE());
        }

        private IEnumerator ConnectToSSE()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(sseURL))
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
    }
}