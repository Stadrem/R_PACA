using System;
using System.Collections;
using Data.Remote.Dtos.Request;

namespace Data.Remote.Api
{
    [System.Serializable]
    public class PlayProgressApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/progress";

        public static IEnumerator StartNpcTalk(int roomNumber, string backgroundName, string npcName, Action<ApiResult> onComplete)
        {
            var reqDto = new StartTalkReqDto()
            {
                roomNumber = roomNumber,
                backgroundName = backgroundName,
                npcName = npcName,
            };

            var request = new HttpInfoWithType<string, StartTalkReqDto>()
            {
                url = $"{BaseUrl}/start",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
                
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}