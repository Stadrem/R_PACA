using System;
using System.Collections;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote
{
    public class ScenarioBackgroundApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/world";

        public static IEnumerator CreateScenarioBackground(
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

        public static IEnumerator UpdateScenarioBackground(
            BackgroundPartInfo backgroundPartInfo,
            Action<ApiResult<ScenarioBackgroundUpdateResDto>> onCompleted
        )
        {
            var reqDto = new ScenarioBackgroundUpdateReqDto()
            {
                partId = backgroundPartInfo.ID,
                partName = backgroundPartInfo.Name,
                isPortalEnable = backgroundPartInfo.TowardBackground != null,
            };
            
            var request = new HttpInfoWithType<ScenarioBackgroundUpdateResDto, ScenarioBackgroundUpdateReqDto>()
            {
                url = $"{BaseUrl}/update",
                body = reqDto,
                onComplete = (result) => onCompleted(ApiResult<ScenarioBackgroundUpdateResDto>.Success(result)),
                onError = (error) => onCompleted(ApiResult<ScenarioBackgroundUpdateResDto>.Fail(error)),
            };
            
            yield return HttpManager.GetInstance().Put(request);
        }
    }
    
}