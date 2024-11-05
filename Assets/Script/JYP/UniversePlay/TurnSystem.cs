using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private int Turn { get; set; }

    private int currentTurnCharacterId;
    
    public int GetNextTurn()
    {
        return ++Turn;
    }

    public void InitTurn()
    {
        Turn = 0;
    }
}