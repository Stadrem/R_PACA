

using System.Collections.Generic;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class ScenarioListResponseDto
    {
        public List<ScenarioListItemResponseDto> data;
    }

    [System.Serializable]
    public class ScenarioListItemResponseDto
    {
        public int scenarioCode;
        public string scenarioTitle;
    }
}