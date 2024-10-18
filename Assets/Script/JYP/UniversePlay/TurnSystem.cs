using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TurnSystem : MonoBehaviour, IPunObservable
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}