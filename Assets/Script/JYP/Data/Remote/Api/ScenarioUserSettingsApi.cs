using System;
using System.Collections;
using Data.Models.Universe.Characters.Player;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Api
{
    public static class ScenarioUserSettingsApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario/user";

        public static IEnumerator UploadUserSettings(UniversePlayerSettings playerSettings, Action<ApiResult> onCompleted)
        {
            var reqDto = playerSettings.ToSecenarioUserUploadDto();

            var request = new HttpInfoWithType<string, ScenarioUserUploadReqDto>()
            {
                url = $"{BaseUrl}/upload",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult.Success()); },
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator GetUserSettings(int userCode, int universeId,
            Action<ApiResult<UniversePlayerSettings>> onCompleted)
        {
            var reqDto = new ScenarioUserGetReqDto()
            {
                userCode = userCode,
                scenarioCode = universeId,
            };

            var request = new HttpInfoWithType<ScenarioUserResponseDto, ScenarioUserGetReqDto>()
            {
                url = $"{BaseUrl}/get",
                body = reqDto,
                onComplete = (result) =>
                {
                    onCompleted(ApiResult<UniversePlayerSettings>.Success(result.ToUniverseUserSettings()));
                },
                onError = (error) => onCompleted(ApiResult<UniversePlayerSettings>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}