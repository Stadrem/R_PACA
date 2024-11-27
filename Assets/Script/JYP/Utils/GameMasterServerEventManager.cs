using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class GameMasterServerEventManager : MonoBehaviour
    {
        private readonly string sseURL = $"{HttpManager.ServerURL}/gm";

        public Action<List<string>> OnEventReceived;


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

        public void DeleteInstance()
        {
            if (instance != null)
            {
                StopAllCoroutines();
                Destroy(instance.gameObject);
                instance = null;
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
            StopAllCoroutines();
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

                    try
                    {
                        string responseText = downloadHandler.GetData();
                        if (!string.IsNullOrEmpty(responseText))
                        {
                            responseText = responseText.Trim();
                            Debug.Log($"SSE Data Received: {responseText}");
                            var parsedData = ParseData(responseText);
                            OnEventReceived?.Invoke(parsedData);
                            downloadHandler.ClearData();
                        }
                    }
                    catch (Exception e)
                    {
                        downloadHandler.ClearData();
                        Debug.LogError($"SSE Error: {e.Message}");
                    }

                    yield return null; // 잠시 대기 후 반복
                }
                Debug.Log("연결 끝!!!!!!");
            }
        }

        public class GameMasterEventDto
        {
            public string message;
        }

        private List<string> ParseData(string data)
        {
            //remove data:
            // var idx = data.IndexOf("{", StringComparison.Ordinal);
            return data.Split("\n\n")
                .Select(ParseEach)
                .Select(x => x.message)
                .ToList();
        }
        
        private GameMasterEventDto ParseEach(string data)
        {
            data = data.Trim();
            var idx = data.IndexOf("{", StringComparison.Ordinal);
            if (idx < 0)
            {
                return null;
            }
            data = data.Substring(idx, data.Length - idx);
            var collection = JsonConvert.DeserializeObject<GameMasterEventDto>(data);
            return collection;
        }
    }
}