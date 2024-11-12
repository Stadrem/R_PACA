namespace Data.Models.Universe.Characters
{
    public class CharacterStats
    {
        public int HitPoints { get; private set; }
        public int Strength { get; private set; }
        public int Dexterity { get; private set; }

        public CharacterStats(int hitPoints, int strength, int dexterity)
        {
            HitPoints = hitPoints;
            Strength = strength;
            Dexterity = dexterity;
        }
    }
}