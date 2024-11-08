using System;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos.Request
{
    [Serializable]
    public class ScenarioBackgroundUpdateReqDto
    {
        public int partId;
        public string partName;
        public bool isPortalEnable;
        public int towardWorldPartId;
    }
}