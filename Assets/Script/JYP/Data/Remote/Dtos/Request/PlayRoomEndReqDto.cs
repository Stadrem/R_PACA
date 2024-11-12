namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class PlayRoomEndReqDto
    {
        public int roomNum;
    }

    public static class PlayRoomEndReqDtoExtension
    {
        public static PlayRoomEndReqDto ToPlayRoomEndReqDto(this int roomNum)
        {
            return new PlayRoomEndReqDto
            {
                roomNum = roomNum
            };
        }
    }
}