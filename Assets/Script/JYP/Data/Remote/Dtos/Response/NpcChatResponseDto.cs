using System.Runtime.CompilerServices;
using Data.Models.Universe.Characters;
using Newtonsoft.Json;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class NpcChatResponseDto
    {
        [JsonProperty("event")]
        public string eventTypeName;

        [JsonProperty("bonus")]
        public string bonus;

        [JsonProperty("npcChat")]
        public string npcChat;
    }

    public static class NpcChatResponseDtoFactory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NpcReaction ToReaction(this NpcChatResponseDto dto)
        {
            var reactionType = EReactionType.None;
            switch (dto.eventTypeName)
            {

                case "전투":
                    reactionType = EReactionType.Battle;
                    break;
                case "다이스":
                    reactionType = EReactionType.Dice;
                    break;
                default:
                    reactionType = EReactionType.Progress;
                    break;
            }
            return new NpcReaction(reactionType, dto.npcChat, dto.bonus);
        }
    }
}