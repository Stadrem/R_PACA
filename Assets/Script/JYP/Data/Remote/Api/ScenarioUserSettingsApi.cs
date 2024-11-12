using System;
using System.Collections;
using Data.Models.Universe.Characters.Player;
using Data.Remote.Dtos.Request;

namespace Data.Remote.Api
{
    public static class ScenarioUserSettingsApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario/user";

        public static IEnumerator UploadUserSettings(UniverseUserSettings userSettings, Action<ApiResult> onCompleted)
        {
            var reqDto = userSettings.ToSecenarioUserUploadDto();

            var request = new HttpInfoWithType<string, ScenarioUserUploadReqDto>()
            {
                url = $"{BaseUrl}/upload",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult.Success()); },
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
        

    }
}
