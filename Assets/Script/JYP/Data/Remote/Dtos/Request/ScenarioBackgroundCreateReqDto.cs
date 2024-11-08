namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class ScenarioBackgroundCreateReqDto
    {
        public string WorldName;

        /// <summary>
        /// if true, can go to next background(map)
        /// </summary>
        public bool isPortalEnable;

        public int towardWorldPartId;
    }
}