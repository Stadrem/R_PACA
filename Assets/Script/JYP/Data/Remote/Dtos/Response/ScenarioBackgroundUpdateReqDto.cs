using System;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos.Response
{
    [Serializable]
    public class ScenarioBackgroundUpdateResDto
    {
        public int partId;
        public string WorldName;
        public int partType;
        public bool isPortalEnable;
        public int towardWorldPartId;
    }
}