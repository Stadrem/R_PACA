namespace Data.Models.Universe.Characters.Player
{
    public class UniversePlayerSettings
    {
        public int UniverseId { get; private set; }

        public int UserCode { get; private set; }

        public CharacterStats CharacterStats { get; private set; }


        public UniversePlayerSettings(int universeId, int userCode, CharacterStats characterStats)
        {
            UniverseId = universeId;
            UserCode = userCode;
            CharacterStats = characterStats;
        }
    }
}