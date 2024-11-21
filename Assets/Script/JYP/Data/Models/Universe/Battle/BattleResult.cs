using System.Collections.Generic;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;

namespace Data.Models.Universe.Battle
{
    /// <summary>
    /// 전투 결과
    /// </summary>
    public class BattleResult
    {
        public int RoomNumber { get; private set; }
        public int IsBattleWon { get; private set; }
        public List<UniversePlayer> Players { get; private set; }
        public UniverseNpc Npc { get; private set; }

        public BattleResult(int roomNumber, int isBattleWon, List<UniversePlayer> players, UniverseNpc npc)
        {
            RoomNumber = roomNumber;
            IsBattleWon = isBattleWon;
            Players = players;
            Npc = npc;
        }
    }
}