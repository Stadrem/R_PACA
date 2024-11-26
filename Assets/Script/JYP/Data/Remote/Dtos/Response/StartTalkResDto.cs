using Newtonsoft.Json;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class StartTalkResDto
    {
        [JsonProperty("OpeningChat")]
        public string npcFirstChat;
    }
}