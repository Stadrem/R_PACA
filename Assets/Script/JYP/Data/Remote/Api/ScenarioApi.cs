using System;
using System.Collections;
using System.Collections.Generic;
using Data.Remote.Dtos.Request;

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
                genre = genre[0],
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
    }
}