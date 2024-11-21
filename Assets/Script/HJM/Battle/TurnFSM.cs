using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTurn
{
    Base = -1,
    Player = 0,
    Enemy = 1,
}

public class TurnFSM : MonoBehaviourPunCallbacks
{
    public ActionTurn turnState = ActionTurn.Player;
    public int finishActionCount = 0;


    private void Update()
    {
        if(turnState == ActionTurn.Player)
        {
            if(finishActionCount >= 2)
            {
                photonView.RPC("ChangeTurn", RpcTarget.All);
                finishActionCount = 0;
            }
        }
    }

    [PunRPC]
    public void ChangeTurn()
    {
        // 턴 대상을 바꿈
        turnState = (ActionTurn)((int)++turnState % 2);
    }
}
