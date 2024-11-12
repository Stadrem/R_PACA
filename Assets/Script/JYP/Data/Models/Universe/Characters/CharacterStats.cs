using System.Collections.Generic;

namespace Data.Models.Universe.Characters
{
    public class CharacterStats
    {
        private readonly Dictionary<string, int> stats = new Dictionary<string, int>();

        public int GetStat(EStatType statType)
        {
            return GetStat(statType.ToString());
        }

        public int GetStat(string statName)
        {
            return stats[statName];
        }


        public CharacterStats(int hitPoints, int strength, int dexterity)
        {
            stats.Add(EStatType.Hp.ToString(), hitPoints);
            stats.Add(EStatType.Str.ToString(), strength);
            stats.Add(EStatType.Dex.ToString(), dexterity);
        }
    }

    public enum EStatType
    {
        Hp,
        Str,
        Dex
    }
}