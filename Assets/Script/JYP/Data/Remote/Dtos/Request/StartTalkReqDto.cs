using Newtonsoft.Json;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class StartTalkReqDto
    {
        [JsonProperty("roomNum")]
        public int roomNumber;
        
        [JsonProperty("location")]
        public string backgroundName;
        
        [JsonProperty("npc")]
        public string npcName;
    }
}