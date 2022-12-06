using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class InGameDisplay : MonoBehaviourPunCallbacks
{
    [Header("Pre-Game")]
    public TextMeshProUGUI waitingTMP;

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
    public void Update()
    {
        // Only the client of the host will update the in-game display. the other client(s) will fetch the data of the in-game display
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateCanvas();
            currentRoom = PhotonNetwork.CurrentRoom;    
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
    }

    void PrepareStartGame()
    {

    }
}
