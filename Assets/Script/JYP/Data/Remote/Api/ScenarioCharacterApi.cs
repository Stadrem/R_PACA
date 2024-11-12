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
            Action<ApiResult> onCompleted
        )
        {
            var reqDto = new ScenarioCharacterUpdateReqDto()
            {
                scenarioAvatarId = characterInfo.id,
                avatarName = characterInfo.name,
                outfit = (int)characterInfo.shapeType,
                isPlayable = characterInfo.isPlayable,
                worldId = characterInfo.backgroundPartId,
                health = characterInfo.hitPoints,
                dex = characterInfo.dexterity,
                strength = characterInfo.strength,
                axisX = characterInfo.position.x,
                axisY = characterInfo.position.y,
                axisZ = characterInfo.position.z,
                rotation = characterInfo.yRotation,
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