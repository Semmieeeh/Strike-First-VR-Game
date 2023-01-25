using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class EndgameManager : MonoBehaviourPunCallbacks
{
    public Transform winnerPos, loserPos;

    public TextMeshProUGUI winnerText, disconnectStatus;
    Transform myPlayer;

    void Start()
    {
        Server.SetMovementActive(false);

        myPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        if(Server.MyPlayer == Server.Winner)
        {
            myPlayer.transform.position = winnerPos.position;
            winnerText.text = Server.MyPlayer.NickName;
        }
        else
        {
            myPlayer.transform.position = loserPos.position;
            winnerText.text = Server.OtherPlayer.NickName;
        }

        StartDisconnect();
    }

    void StartDisconnect()
    {
        Invoke(nameof(DisconnectToServer),8f);
    }

    void DisconnectToServer()
    {
        disconnectStatus.text = "Leaving Room...";
        PhotonNetwork.LeaveRoom();
        print("leaving room");
    }

    public override void OnLeftRoom()
    {
        print("room left!");
        base.OnLeftRoom();

        disconnectStatus.text = "Leaving Lobby...";
        PhotonNetwork.LeaveLobby();
        print("Leaving Lobby");
    }

    public override void OnLeftLobby()
    {
        print("Lobby Left!");
        base.OnLeftLobby();

        disconnectStatus.text = "Loading Main Menu...";
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
        print("Loading Main Menu");
    }
}
