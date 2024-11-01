using System;

namespace Data.Remote.Dtos.Response
{
    [Serializable]
    public class ScenarioBackgroundCreateResDto
    {
        public int partId;
        public string partName;
        public bool isPortalEnable;
    }
}
