using System.Collections.Generic;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
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

    [System.Serializable]
    public class ScenarioAvatarResDto
    {
        public int avatarCode;
        public string avatarName;
        public string avatarImage;
    }

    [System.Serializable]
    public class ScenarioWorldPartResDto
    {
        public int partId;
        public string partName;
        public string isPortalEnable;
    }


    [System.Serializable]
    public class GenreResDto
    {
        public int genreCode;
        public string genreName;
    }

    [System.Serializable]
    public class TagResDto
    {
        public int tagCode;
        public string tagName;
    }
}