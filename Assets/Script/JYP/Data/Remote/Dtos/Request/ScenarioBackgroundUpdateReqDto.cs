using System;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos.Request
{
    [Serializable]
    public class ScenarioBackgroundUpdateReqDto
    {
        public int partId;
        public string WorldName;
        public int partType;
        public bool isPortalEnable;
        public int towardWorldPartId;
    }
}