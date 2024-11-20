using System;
using System.Collections;
using Data.Models.Universe.Battle;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Dice;
using Data.Remote.Dtos.Request;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Api
{
    /// <summary>
    /// 생성된 게임방 내부에서 NPC와 대화를 주고받을 때 발생하는 이벤트들에 대한
    /// </summary>
    public class PlayProgressApi
    {
        private static readonly string BaseUrl = $"{HttpManager.ServerURL}/progress";

        public static IEnumerator StartNpcTalk(int roomNumber, string backgroundName, string npcName,
            Action<ApiResult> onComplete)
        {
            var reqDto = new StartTalkReqDto()
            {
                roomNumber = roomNumber,
                backgroundName = backgroundName,
                npcName = npcName,
            };

            var request = new HttpInfoWithType<string, StartTalkReqDto>()
            {
                url = $"{BaseUrl}/start",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator SendChat(int roomNumber, string userChat,
            Action<ApiResult<NpcChatResponseDto>> onComplete)
        {
            var reqDto = new PlayProgressSendReqDto()
            {
                roomNum = roomNumber,
                userChat = userChat,
            };

            var request = new HttpInfoWithType<NpcChatResponseDto, PlayProgressSendReqDto>() //todo: response check

            {
                url = $"{BaseUrl}/send",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult<NpcChatResponseDto>.Success(result)); },
                onError = (res) => { onComplete(ApiResult<NpcChatResponseDto>.Fail(res)); },
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator CheckDice(int roomNumber, DiceResult diceResult,
            Action<ApiResult<NpcReaction>> onComplete)
        {
            var reqDto = new CheckDiceReqDto()
            {
                roomNum = roomNumber,
                diceFst = diceResult.FirstDiceNumber,
                diceSnd = diceResult.SecondDiceNumber,
            };


            var request = new HttpInfoWithType<NpcChatResponseDto, CheckDiceReqDto>() // TODO: response check
            {
                url = $"{BaseUrl}/dice",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult<NpcReaction>.Success(result.ToReaction())); },
                onError = (error) => onComplete(ApiResult<NpcReaction>.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator
            FinishNpcTalk(int roomNumber, Action<ApiResult> onComplete) // todo: JYP가 임의로 작성한 request body임, 수정 필요
        {
            var reqDto = new PlayProgressEndReqDto()
            {
                roomNum = roomNumber,
            };

            var request = new HttpInfoWithType<string, PlayProgressEndReqDto>()
            {
                url = $"{BaseUrl}/end",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator SendBattleResult(BattleResult battleResult, Action<ApiResult> onComplete)
        {
            var reqDto = battleResult.ToDto();

            var request = new HttpInfoWithType<string, PlayProgressBattleResultReqDto>()
            {
                url = $"{BaseUrl}/battle",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }
    }
}