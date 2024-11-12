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
            
            return new NpcReaction(EReactionType.Progress, dto.npcChat, dto.bonus);
        }
    }
}