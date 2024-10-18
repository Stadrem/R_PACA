using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnSystem : MonoBehaviour
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