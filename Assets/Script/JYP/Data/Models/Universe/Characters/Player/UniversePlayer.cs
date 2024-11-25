namespace Data.Models.Universe.Characters.Player
{
    public class UniversePlayer
    {
        public int UserCode { get; private set; }
        public string PlayerId { get; private set; }
        public string Nickname { get; private set; }
        public CharacterStats Stats { get; set; }

        public int currentHp;

        public UniversePlayer(string playerId,string nickname, int userCode, CharacterStats stats)
        {
            UserCode = userCode;
            Nickname = nickname;
            Stats = stats;
            PlayerId = playerId;
        }
    }
}