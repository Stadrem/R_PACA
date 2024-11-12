namespace Data.Models.Universe.Characters.Player
{
    public class UniverseUserSettings
    {
        public int UniverseId { get; private set; }

        public int UserId { get; private set; }

        public CharacterStats CharacterStats { get; private set; }


        public UniverseUserSettings(int universeId, int userId, CharacterStats characterStats)
        {
            UniverseId = universeId;
            UserId = userId;
            CharacterStats = characterStats;
        }
    }
}