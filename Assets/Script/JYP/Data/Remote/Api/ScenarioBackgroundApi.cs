using System;
using System.Collections;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote
{
    public static class ScenarioBackgroundApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/world";

        public static IEnumerator CreateScenarioBackground(
            BackgroundPartInfo backgroundInfo,
            Action<ApiResult<ScenarioBackgroundCreateResDto>> onCompleted
        )
        {
            var reqDto = new ScenarioBackgroundCreateReqDto()
            {
                backgroundName = backgroundInfo.Name,
                backgroundType = (int)backgroundInfo.Type,
                isPortalEnable = false,
                towardWorldPartId = -1
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
                WorldName = backgroundPartInfo.Name,
                partType = (int)backgroundPartInfo.Type,
                isPortalEnable = backgroundPartInfo.TowardBackground != null,
                towardWorldPartId = backgroundPartInfo.TowardBackground?.ID ?? -1
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

        public static IEnumerator DeleteScenarioBackground(
            int backgroundPartId,
            Action<ApiResult> onCompleted
        )
        {
            var reqDto = new ScenarioBackgroundDeleteReqDto()
            {
                worldId = backgroundPartId,
            };

            var request = new HttpInfoWithType<string, ScenarioBackgroundDeleteReqDto>()
            {
                url = $"{BaseUrl}/delete",
                body = reqDto,
                onComplete = (result) => onCompleted(ApiResult.Success()),
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Delete(request);
        }
    }
}