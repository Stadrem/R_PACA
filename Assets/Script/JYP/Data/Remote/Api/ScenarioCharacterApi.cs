using System;
using System.Collections;
using Data.Remote.Dtos;
using Data.Remote.Dtos.Response;
using UnityEngine;

namespace Data.Remote
{
    public static class ScenarioCharacterApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/scenario/avatar";

        public static IEnumerator CreateScenarioAvatar(
            CharacterInfo characterInfo,
            Action<ApiResult<CharacterInfo>> onCompleted
        )
        {
            var reqDto = new ScenarioCharacterCreateReqDto()
            {
                avatarName = characterInfo.name,
                outfit = (int)characterInfo.shapeType,
                isPlayable = false,
                health = characterInfo.hitPoints,
                strength = characterInfo.strength,
                dex = characterInfo.dexterity,
                worldId = -1,
            };

            var request = new HttpInfoWithType<ScenarioCharacterUpdateResDto, ScenarioCharacterCreateReqDto>()
            {
                url = $"{BaseUrl}/create",
                body = reqDto,
                onComplete = (result) => { onCompleted(ApiResult<CharacterInfo>.Success(result.ToCharacterInfo())); },
                onError = (error) => onCompleted(ApiResult<CharacterInfo>.Fail(error)),
            };


            yield return HttpManager.GetInstance().Post(request);
        }


        public static IEnumerator UpdateScenarioAvatar(
            CharacterInfo characterInfo,
            int worldId,
            Vector3 position,
            float yRotation,
            Action<ApiResult> onCompleted
        )
        {
            var reqDto = new ScenarioCharacterUpdateReqDto()
            {
                avatarName = characterInfo.name,
                outfit = (int)characterInfo.shapeType,
                isPlayable = characterInfo.isPlayable,
                worldId = worldId,
                axisX = position.x,
                axisY = position.y,
                axisZ = position.z,
                rotation = yRotation,
            };

            var request = new HttpInfoWithType<ScenarioCharacterUpdateResDto, ScenarioCharacterUpdateReqDto>()
            {
                url = $"{BaseUrl}/update",
                body = reqDto,
                onComplete = (result) => onCompleted(ApiResult.Success()),
                onError = (error) => onCompleted(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Put(request);
        }

        public static IEnumerator DeleteScenarioAvatar(
            int scenarioAvatarId,
            Action<ApiResult> onCompleted
        )
        {
            var reqDto = new ScenarioCharacterDeleteReqDto()
            {
                avatarId = scenarioAvatarId,
            };

            var request = new HttpInfoWithType<string, ScenarioCharacterDeleteReqDto>()
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