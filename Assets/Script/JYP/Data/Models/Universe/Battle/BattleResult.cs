using System.Collections.Generic;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;

namespace Data.Models.Universe.Battle
{
    public class BattleResultPlayer
    {
        public int UserCode { get; private set; }
        public int CurrentHp { get; private set; }
        
        public BattleResultPlayer(int userCode, int currentHp)
        {
            UserCode = userCode;
            CurrentHp = currentHp;
        }
    }
    
    public class BattleResultNpc
    {
        public int NpcId { get; private set; }
        public int CurrentHp { get; private set; }
        
        public BattleResultNpc(int npcId, int currentHp)
        {
            this.NpcId = npcId;
            this.CurrentHp = currentHp;
        }
    }
    /// <summary>
    /// 전투 결과
    /// </summary>
    public class BattleResult
    {
        public int RoomNumber { get; private set; }
        public bool IsBattleWon { get; private set; }
        public List<BattleResultPlayer> Players { get; private set; }
        public BattleResultNpc Npc { get; private set; }

        public BattleResult(int roomNumber, bool isBattleWon,List<BattleResultPlayer> players, BattleResultNpc npc)
        {
            RoomNumber = roomNumber;
            IsBattleWon = isBattleWon;
            Players = players;
            Npc = npc;
        }
    }
}