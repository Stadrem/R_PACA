using System.Collections.Generic;

public class TurnSystem
{
    public int Turn { get; private set; }

    private int currentTurnCharacterId;
    
    public void NextTurn(int id)
    {
        Turn++;
    }
    
    public void InitTurn(List<NpcInPlay> players)
    {
        Turn = 0;
    }
}