namespace Data.Models.Universe.Dice
{
    public class DiceResult
    {
        public int FirstDiceNumber { get; private set; }
        public int SecondDiceNumber { get; private set; }

        public int UserCode { get; private set; }

        public DiceResult(int userCode, int firstDiceNumber, int secondDiceNumber)
        {
            UserCode = userCode;
            FirstDiceNumber = firstDiceNumber;
            SecondDiceNumber = secondDiceNumber;
        }
    }
}