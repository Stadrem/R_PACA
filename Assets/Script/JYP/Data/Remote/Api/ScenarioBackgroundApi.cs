using System;
using System.Collections;
using Data.Remote.Dtos.Request;

namespace Data.Remote
{
    public class ScenarioBackgroundApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/world";

        public static IEnumerator CreateScenarioMap(
            BackgroundPartInfo backgroundInfo,
            Action<ApiResult<int>> onCompleted
        )
        {
            var isPortalEnabled = backgroundInfo.portalList.Count > 0;
            var reqDto = new ScenarioBackgroundCreateReqDto()
            {
                WorldName = backgroundInfo.Name,
                isPortalEnabled = isPortalEnabled,
            };

            var request = new HttpInfoWithType<int, ScenarioBackgroundCreateReqDto>()
            {
                url = $"{BaseUrl}/create",
                body = reqDto,
                onComplete = (result) => onCompleted(ApiResult<int>.Success(result)),
                onError = (error) => onCompleted(ApiResult<int>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}