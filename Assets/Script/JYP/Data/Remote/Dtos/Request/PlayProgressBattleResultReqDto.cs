using System;
using System.Collections.Generic;
using Data.Models.Universe.Battle;
using Data.Models.Universe.Characters;
using Newtonsoft.Json;

namespace Data.Remote.Dtos.Request
{
    /// <summary>
    /// 전투 결과를 요청하는 DTO
    /// </summary>
    [Serializable]
    public class PlayProgressBattleResultReqDto
    {
        [JsonProperty("roomNum")]
        public int roomNumber;

        [JsonProperty("isBattleWon")]
        public bool isBattleWon;

        [JsonProperty("userSatusList")]
        public List<UserStatusDto> userStatusList;

        [JsonProperty("npcStatusDTO")]
        public NpcDto npcStatus;

        [Serializable]
        public class UserStatusDto
        {
            [JsonProperty("userCode")]
            public int userCode;

            [JsonProperty("healthPoint")]
            public int hitPoint;

            [JsonProperty("status")]
            public List<string> status;
        }

        [Serializable]
        public class NpcDto
        {
            [JsonProperty("scenarioAvatarId")]
            public int npcId;

            [JsonProperty("healthPoint")]
            public int hitPoint;

            [JsonProperty("status")]
            public List<string> status;
        }
    }

    public static class PlayProgressBattleResultReqDtoExtension
    {
        public static PlayProgressBattleResultReqDto ToDto(this BattleResult battleResult)
        {
            var userStatusList = new List<PlayProgressBattleResultReqDto.UserStatusDto>();
            foreach (var player in battleResult.Players)
            {
                var userStatus = new PlayProgressBattleResultReqDto.UserStatusDto()
                {
                    userCode = player.UserCode,
                    hitPoint = player.CurrentHp,
                    status = new List<string>()
                };
                userStatusList.Add(userStatus);
            }

            var npcStatus = new PlayProgressBattleResultReqDto.NpcDto()
            {
                npcId = battleResult.Npc.NpcId,
                hitPoint = battleResult.Npc.CurrentHp,
                status = new List<string>()
            };

            return new PlayProgressBattleResultReqDto()
            {
                roomNumber = battleResult.RoomNumber,
                isBattleWon = battleResult.IsBattleWon,
                userStatusList = userStatusList,
                npcStatus = npcStatus
            };
        }
    }
}