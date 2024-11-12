using System;
using System.Collections;
using Data.Models.Universe.Dice;
using Data.Remote.Dtos.Request;

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

        public static IEnumerator SendChat(int roomNumber, string userChat, Action<ApiResult> onComplete)
        {
            var reqDto = new PlayProgressSendReqDto()
            {
                roomNum = roomNumber,
                userChat = userChat,
            };

            var request = new HttpInfoWithType<string, PlayProgressSendReqDto>() //todo: response check

            {
                url = $"{BaseUrl}/send",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };

            yield return HttpManager.GetInstance().Post(request);
        }

        public static IEnumerator CheckDice(int roomNumber, DiceResult diceResult, Action<ApiResult> onComplete)
        {
            var reqDto = new CheckDiceReqDto()
            {
                roomNum = roomNumber,
                diceFst = diceResult.FirstDiceNumber,
                diceSnd = diceResult.SecondDiceNumber,
            };
            
            
            var request = new HttpInfoWithType<string, CheckDiceReqDto>() // TODO: response check
            {
                url = $"{BaseUrl}/dice",
                body = reqDto,
                onComplete = (result) => { onComplete(ApiResult.Success()); },
                onError = (error) => onComplete(ApiResult.Fail(error)),
            };
            
            yield return HttpManager.GetInstance().Post(request);
        }
    }
}