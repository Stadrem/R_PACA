using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class ScenarioBackgroundCreateReqDto
    {
        [JsonProperty("WorldName")]
        public string backgroundName;

        [JsonProperty("partType")]
        public int backgroundType;

        /// <summary>
        /// if true, can go to next background(map)
        /// </summary>
        public bool isPortalEnable;

        public int towardWorldPartId;
    }
}