using System.Collections.Generic;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class PlayRoomStartReqDto
    {
        public int roomNum;
        public string roomTitle;
        public int scenarioId;
        public List<int> userCodes;
    }
}