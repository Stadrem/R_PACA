using System;
using System.Collections;
using System.Collections.Generic;
using Data.Remote.Dtos.Request;

namespace Data.Remote.Api
{
    public static class PlayRoomApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/room";

        public static IEnumerator StartRoom(int roomNum, int universeId, List<int> playerIds,
            Action<ApiResult> onCompleted)
        {
            var reqDto = new PlayRoomStartReqDto()
            {
                roomNum = roomNum,
                scenarioId = universeId,
                userCodes = playerIds,
            };

            var request = new HttpInfoWithType<string, PlayRoomStartReqDto>()
            {
                url = $"{BaseUrl}/start",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult.Success()); },
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}