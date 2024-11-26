using Newtonsoft.Json;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class PlayRoomStartResDto
    {
        [JsonProperty("gm_msg")]
        public string startRoomIntroMessage;
    }
}