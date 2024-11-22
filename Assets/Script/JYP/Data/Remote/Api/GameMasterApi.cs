using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Remote.Api
{
    public static class GameMasterApi
    {

        private const string TestUrl = "http://125.132.216.190:9876";
        public static IEnumerator SubscribeGameMasterSSE(int roomId, Action<ApiResult> onComplete)
        {
            var request = new HttpInfoWithType<string, string>()
            {
                url = $"{TestUrl}/gm/connect",
                acceptContentType = "text/event-stream",
                parameters = new Dictionary<string, string>()
                {
                    {"roomId", roomId.ToString()}  
                },
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);    
        }
        
        public static IEnumerator UnsubscribeGameMasterSSE(int roomId, Action<ApiResult> onComplete)
        {
            var request = new HttpInfoWithType<string, string>()
            {
                url = $"{TestUrl}/gm/disconnect",
                parameters = new Dictionary<string, string>()
                {
                    {"roomId", roomId.ToString()}  
                },
                acceptContentType = "text/event-stream",
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);    
        }   
        
        public static IEnumerator Test(int roomId, Action<ApiResult> onComplete)
        {
            var request = new HttpInfoWithType<string, string>()
            {
                url = $"{TestUrl}/gm/test",
                parameters = new Dictionary<string, string>()
                {
                    {"roomId", roomId.ToString()}  
                },
                acceptContentType = "text/event-stream",
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);    
        }
    }
}