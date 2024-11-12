namespace Data.Models.Universe.Characters
{
    public class NpcReaction
    {
        public EReactionType ReactionType { get; private set; }
        public string DialogMessage { get; private set; }
        public string BonusMessage { get; private set; }

        public NpcReaction(EReactionType reactionType, string dialogMessage, string bonusMessage)
        {
            ReactionType = reactionType;
            DialogMessage = dialogMessage;
            BonusMessage = bonusMessage;
        }
    }

    public enum EReactionType
    {
        None,
        Progress, // 진행 
        Battle,     // 전투 
        Dice   // 다이스
    }
}