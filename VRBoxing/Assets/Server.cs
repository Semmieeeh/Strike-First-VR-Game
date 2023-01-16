using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Server : MonoBehaviourPunCallbacks
{
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player entered the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    private void Update()
    {
        if (MyPlayer == null)
        {
            MyPlayer = PhotonNetwork.LocalPlayer;
        }
        if (OtherPlayer == null)
        {
            OtherPlayer = PhotonNetwork.PlayerListOthers[0];
        }
        print(MyPlayer);
        print(OtherPlayer);
    }

    public static Player MyPlayer;
    public static Player OtherPlayer;
}
