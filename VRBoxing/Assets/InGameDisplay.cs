using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Threading;
using System.Threading.Tasks;

public class InGameDisplay : MonoBehaviourPunCallbacks
{
    [Header("Pre-Game")]
    public TextMeshProUGUI waitingTMP;

    public TextMeshProUGUI player1, player2;
    public float timeToWaitWhenGameCanStart;

    public int roundsToPlay;

    Room currentRoom;

    public bool LobbyFull
    {
        get
        {
            return currentRoom.PlayerCount >= currentRoom.MaxPlayers;
        }
    }

    bool gameStarted;
    bool prepareGameStarted;

    public bool playerDied;
    public void Update()
    {
        // Only the client of the host will update the in-game display. the other client(s) will fetch the data of the in-game display
        if (PhotonNetwork.IsMasterClient)
        {
            currentRoom = PhotonNetwork.CurrentRoom;    
            UpdateCanvas();
        }
    }

    public void UpdateCanvas()
    {
        if (currentRoom == null) return;

        //Update Waiting for players text
        waitingTMP.text = "Waiting for players: " + currentRoom.PlayerCount + " / " + currentRoom.MaxPlayers;

        //Check if another player has joined
        if (LobbyFull && prepareGameStarted == false)
        {
            PrepareStartGame();
        }

        //update the players name
        if (Server.MyPlayer != null)
        {
            player1.text = Server.MyPlayer.NickName;
        }
        if(Server.OtherPlayer != null)
        {
            player2.text = Server.OtherPlayer.NickName;
        }
    }

    void PrepareStartGame()
    {
        print("Game Can Start!");
        timeToWaitWhenGameCanStart -= Time.deltaTime;
        if(timeToWaitWhenGameCanStart <= 0)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        for (int i = 0; i < roundsToPlay; i++)
        {

        }
    }

}
