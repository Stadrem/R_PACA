namespace UniversePlay
{
    public interface ISelectorChat
    {
        /// <summary>
        /// 현재 선택지 가짓수
        /// </summary>
        int OptionCount { get; }

        /// <summary>
        /// 선택지 추가
        /// </summary>
        /// <param name="option"></param>
        void AddOption(string option);

        /// <summary>
        /// 선택지 표시
        /// </summary>
        void ShowSelectors();

        /// <summary>
        /// 선택지 선택
        /// </summary>
        /// <param name="index">선택지 index</param>
        void Select(int index);

        /// <summary>
        /// 커스텀 선택지선택
        /// </summary>
        /// <param name="sender">말하는 사람(마스터)</param>
        /// <param name="option">선택지 대사/액션</param>
        void Select(string sender, string option);

        void ClearOptions();
    }
}