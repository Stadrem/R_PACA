using System;
using System.Collections;

namespace Data.Remote
{
    public class ScenarioAvatarApi
    {
        public static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario/avatar";

        public static IEnumerator CreateScenarioAvatar(
            CharacterInfo characterInfo,
            Action<ApiResult<int>> onCompleted
        )
        {
            var reqDto = new ScenarioAvatarCreateReqDto()
            {
                avatarName = characterInfo.name,
                outfit = (int)characterInfo.shapeType,
                isPlayable = characterInfo.isPlayable,
            };

            var request = new HttpInfoWithType<int, ScenarioAvatarCreateReqDto>()
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