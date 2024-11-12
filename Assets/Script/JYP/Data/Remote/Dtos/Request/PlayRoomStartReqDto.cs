using System.Collections.Generic;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class PlayRoomStartReqDto
    {
        public int roomNum;
        public int scenarioId;
        public List<int> userCodes;
    }
}