using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EndgameManager : MonoBehaviourPunCallbacks
{
    public Transform winnerPos, loserPos;

    Transform myPlayer;
    void Start()
    {
        myPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        if(Server.MyPlayer == Server.Winner)
        {
            myPlayer.transform.position = winnerPos.position;
        }
        else
        {
            myPlayer.transform.position = loserPos.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
