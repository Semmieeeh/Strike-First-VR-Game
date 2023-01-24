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

    public GameObject preGameObject;

    [Header("In Game")]
    public RoundData[] rounds;
    public GameObject inGameObject;

    public TextMeshProUGUI player1InGame, player1Health,player1RoundsWon;

    public TextMeshProUGUI player2InGame, player2Health, player2RoundsWon;

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
        //Pre Game 
        if (currentRoom == null) return;

        //Update Waiting for players text
        waitingTMP.text = "Waiting for players: " + currentRoom.PlayerCount + " / " + currentRoom.MaxPlayers;

        //Check if another player has joined
        if (LobbyFull && prepareGameStarted == false)
        {
            PrepareStartGame();
        }

        //update the players name
        SetPlayerNamesPreGame();


        if (timeToWaitWhenGameCanStart <= 0)
        {
            //In Game
            //Update Player 1 settings
            var player1Properties = Server.MyPlayer.CustomProperties;
            player1InGame.text = Server.MyPlayer.NickName;
            player1Health.text = player1Properties[Server.kHealth].ToString() + "%";
            player1RoundsWon.text = player1Properties[Server.kRoundsWon].ToString() + "Rounds Won";

            //Update Player 2 settings
            var player2Properties = Server.OtherPlayer.CustomProperties;
            player2InGame.text = Server.OtherPlayer.NickName;
            player2Health.text = player2Properties[Server.kHealth].ToString() + "%";
            player2RoundsWon.text = player2Properties[Server.kRoundsWon].ToString() + "Rounds Won";
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
        preGameObject.SetActive(false);
        inGameObject.SetActive(true);
        for (int i = 0; i < rounds.Length; i++)
        {

        }
    }

    [PunRPC]
    void SetPlayerNamesPreGame()
    {
        if (Server.MyPlayer != null)
        {
            this.player1.text = Server.MyPlayer.NickName;
        }
        else
        {
            player1.text = "Waiting...";
        }
        if (Server.OtherPlayer != null)
        {
            this.player2.text = Server.OtherPlayer.NickName;
        }
        else
        {
            player2.text = "Waiting...";
        }
    }

    [System.Serializable]
    public struct RoundData
    {
        public float time;
        public Transform player1Pos, player2Pos;

        public string playerThatWon;
        public bool roundOver;
    }

}
