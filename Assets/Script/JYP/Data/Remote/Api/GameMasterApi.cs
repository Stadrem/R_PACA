using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Remote.Api
{
    public static class GameMasterApi
    {
        // private static readonly string testURL = $"http://125.132.216.190:9876";

        public static IEnumerator Test(int roomId, Action<ApiResult> onComplete)
        {
            var request = new HttpInfoWithType<string, string>()
            {
                url = $"{HttpManager.ServerURL}/gm/test",
                parameters = new Dictionary<string, string>()
                {
                    { "roomId", roomId.ToString() }
                },
                acceptContentType = "text/event-stream",
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);
        }
    }
}