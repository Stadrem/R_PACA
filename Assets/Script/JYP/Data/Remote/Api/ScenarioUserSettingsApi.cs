using System;
using System.Collections;
using System.Collections.Generic;
using Data.Models.Universe.Characters.Player;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Api
{
    public static class ScenarioUserSettingsApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario/user";

        public static IEnumerator UploadUserSettings(UniversePlayerSettings playerSettings,
            Action<ApiResult> onCompleted)
        {
            var reqDto = playerSettings.ToSecenarioUserUploadDto();

            var request = new HttpInfoWithType<string, ScenarioUserUploadReqDto>()
            {
                url = $"{BaseUrl}/upload",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult.Success()); },
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance()
                .Post(request);
        }

        public static IEnumerator GetUserSetting(int userCode,
            int universeId,
            Action<ApiResult<UserSettingResDto>> onComplete)
        {
            var request = new HttpInfoWithType<UserSettingResDto, ScenarioUserGetReqDto>()
            {
                url = $"{BaseUrl}/get",
                parameters = new Dictionary<string, string>()
                {
                    { "userCode", userCode.ToString() },
                    { "scenarioCode", universeId.ToString() },
                },
                onComplete = (result) => onComplete(ApiResult<UserSettingResDto>.Success(result)),
                onError = (error) => onComplete(ApiResult<UserSettingResDto>.Fail(error)),
            };

            yield return HttpManager.GetInstance()
                .Get(request);
        }
    }
}