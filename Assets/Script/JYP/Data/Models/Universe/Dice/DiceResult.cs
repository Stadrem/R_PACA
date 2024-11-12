namespace Data.Models.Universe.Dice
{
    public class DiceResult
    {
        
        public int FirstDiceNumber { get; private set; }
        public int SecondDiceNumber { get; private set; }

        public DiceResult(int firstDiceNumber, int secondDiceNumber)
        {
            FirstDiceNumber = firstDiceNumber;
            SecondDiceNumber = secondDiceNumber;
        }
    }
}