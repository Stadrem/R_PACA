using System;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos.Response
{
    [Serializable]
    public class ScenarioBackgroundUpdateResDto
    {
        public int partId;
        public string partName;
        public bool isPortalEnable;
        public int towardWorldPartId;
    }
}