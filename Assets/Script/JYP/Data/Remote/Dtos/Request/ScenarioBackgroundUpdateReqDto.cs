using System;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos.Request
{
    [Serializable]
    public class ScenarioBackgroundUpdateReqDto
    {
        //TODO: 쓸거면 좀 손봐야됨
        public int partId;
        public string partName;
        public bool isPortalEnable;
        public int towardWorldPartId;
    }
}