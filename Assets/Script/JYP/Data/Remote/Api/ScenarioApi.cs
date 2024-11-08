using System;
using System.Collections;
using System.Collections.Generic;
using Data.Remote.Dtos;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;
using Unity.VisualScripting;

namespace Data.Remote
{
    public class ScenarioApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario";

        public static IEnumerator CreateUniverse(
            string title,
            string mainQuest,
            string detail,
            List<string> genre,
            List<int> scenarioAvatarList,
            List<BackgroundPartInfo> backgroundParts,
            List<string> subQuests,
            List<string> tags,
            Action<ApiResult<string>> onCompleted
        )
        {
            var requestDto = new ScenarioCreateRequestDto()
            {
                scenarioTitle = title,
                mainQuest = mainQuest,
                detail = detail,
                genre = genre,
                worldParts = backgroundParts.ConvertAll(part => part.ID),
                scenarioAvatarList = scenarioAvatarList,
                subQuest = subQuests,
                tags = tags,
            };

            var request = new HttpInfoWithType<string, ScenarioCreateRequestDto>()
            {
                url = $"{BaseUrl}/create",
                body = requestDto,
                onComplete = (result) => onCompleted(ApiResult<string>.Success(result)),
                onError = (error) => onCompleted(ApiResult<string>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator GetScenarioList(
            Action<ApiResult<List<ScenarioListItemResponseDto>>> onCompleted
        )
        {
            var request = new HttpInfoWithType<ScenarioListResponseDto, string>()
            {
                url = $"{BaseUrl}/list",
                onComplete = (result) => onCompleted(ApiResult<List<ScenarioListItemResponseDto>>.Success(result.data)),
                onError = (error) => onCompleted(ApiResult<List<ScenarioListItemResponseDto>>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);
        }

        public static IEnumerator GetScenario(int scenarioId, Action<ApiResult<UniverseData>> onComplete)
        {
            var request = new HttpInfoWithType<ScenarioResDto, ScenarioGetReqDto>()
            {
                body = new ScenarioGetReqDto()
                {
                    scenarioCode = scenarioId
                },
                url = $"{BaseUrl}/{scenarioId}",
                onComplete = (result) => onComplete(ApiResult<UniverseData>.Success(result.ToUniverse())),
                onError = (error) => onComplete(ApiResult<UniverseData>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Get(request);
        }
    }
}