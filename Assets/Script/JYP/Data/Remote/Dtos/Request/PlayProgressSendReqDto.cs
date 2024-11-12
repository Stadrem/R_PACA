using System;

namespace Data.Remote.Dtos.Request
{
    [Serializable]
    public class PlayProgressSendReqDto
    {
        public int roomNum;
        public string userChat;
    }
}