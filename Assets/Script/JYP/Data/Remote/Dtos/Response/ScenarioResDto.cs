using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.Remote.Dtos.Response
{
    [Serializable]
    public class ScenarioResDto
    {
        public int scenarioCode;
        public string scenarioTitle;
        public string mainQuest;
        public List<string> subQuest;
        public string detail;
        public List<ScenarioAvatarResDto> scenarioAvatarList;
        public List<ScenarioWorldPartResDto> worldParts;
        public List<GenreResDto> genre;
        public List<TagResDto> tags;
    }

    [Serializable]
    public class ScenarioAvatarResDto
    {
        public int scenarioAvatarId;
        public string avatarName;
        public int outfit;
        public bool isPlayable;
        public int health;
        public int strength;
        public int dex;
        public float axisX;
        public float axisY;
        public float axisZ;
        public float rotation;
        public int worldId;
    }

    [Serializable]
    public class ScenarioWorldPartResDto
    {
        public int partId;
        public string partName;

        [JsonProperty("partType")]
        public int backgroundType;
        public string isPortalEnable;
        public int towardWorldPartId;
    }


    [Serializable]
    public class GenreResDto
    {
        public int genreCode;
        public string genreName;
    }

    [Serializable]
    public class TagResDto
    {
        public int tagCode;
        public string tagName;
    }
}