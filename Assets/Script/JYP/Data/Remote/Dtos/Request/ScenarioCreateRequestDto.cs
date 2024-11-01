using System;
using System.Collections.Generic;

namespace Data.Remote.Dtos.Request
{
    /// <summary>
    /// 세계관 생성 API 요청 DTO
    /// </summary>
    [Serializable]
    public class ScenarioCreateRequestDto
    {
        public string scenarioTitle;
        public string mainQuest;
        public List<string> subQuest;
        public string detail;
        public List<int> scenarioAvatarList;
        public List<int> worldParts;
        public List<string> genre;
        public List<string> tags;
    }
}