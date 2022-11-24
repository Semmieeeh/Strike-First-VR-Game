using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerData : MonoBehaviourPunCallbacks
{
    public string roomName;

    public void JoinServer()
    {
        PhotonNetwork.JoinRoom(roomName);
        GameManager.MainMenu.PlayerJoinedRoom(this);
    }

}
