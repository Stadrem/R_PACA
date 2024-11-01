using System;
using System.Collections;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote
{
    public class ScenarioBackgroundApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/world";

        public static IEnumerator CreateScenarioMap(
            BackgroundPartInfo backgroundInfo,
            Action<ApiResult<ScenarioBackgroundCreateResDto>> onCompleted
        )
        {
            var reqDto = new ScenarioBackgroundCreateReqDto()
            {
                WorldName = backgroundInfo.Name,
                isPortalEnable = false,
            };

            var request = new HttpInfoWithType<ScenarioBackgroundCreateResDto, ScenarioBackgroundCreateReqDto>()
            {
                url = $"{BaseUrl}/create",
                body = reqDto,
                onComplete = (result) => onCompleted(ApiResult<ScenarioBackgroundCreateResDto>.Success(result)),
                onError = (error) => onCompleted(ApiResult<ScenarioBackgroundCreateResDto>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}