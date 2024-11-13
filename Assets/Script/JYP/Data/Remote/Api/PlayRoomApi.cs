using System;
using System.Collections;
using System.Collections.Generic;
using Data.Remote.Dtos.Request;

namespace Data.Remote.Api
{
    public static class PlayRoomApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/room";

        public static IEnumerator StartRoom(int roomNum,string title, int universeId, List<int> playerIds,
            Action<ApiResult> onCompleted)
        {
            var reqDto = new PlayRoomStartReqDto()
            {
                roomNum = roomNum,
                roomTitle = title,
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

        public static IEnumerator FinishRoom(int roomNum, Action<ApiResult> onCompleted)
        {
            var reqDto = roomNum.ToPlayRoomEndReqDto();
        

            var request = new HttpInfoWithType<string, PlayRoomEndReqDto>()
            {
                url = $"{BaseUrl}/end",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult.Success()); },
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}