using Newtonsoft.Json;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class CheckDiceReqDto
    {
        [JsonProperty("roomNum")]
        public int roomNumber;

        [JsonProperty("diceFst")]
        public int diceResult1;

        [JsonProperty("diceSnd")]
        public int diceResult2;

        [JsonProperty("userCode")]
        public int userCode;

        [JsonProperty("bonus")]
        public string requestStat;
    }
}