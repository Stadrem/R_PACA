

using System.Collections.Generic;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class ScenarioListResponseDto
    {
        public List<ScenarioListItemResponseDto> scenarios;
    }

    [System.Serializable]
    public class ScenarioListItemResponseDto
    {
        public int scenarioCode;
        public string scenarioTitle;
    }
}