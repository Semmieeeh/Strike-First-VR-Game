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

    public TextMeshProUGUI countdown;

    [Header("Round Celebration")]
    public GameObject roundWonSpotlight;

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

    async void StartGame()
    {
        prepareGameStarted = true;
        preGameObject.SetActive(false);
        inGameObject.SetActive(true);

        var myPlayer = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < rounds.Length; i++)
        {
            var round = rounds[i];

            //set player on the right spot
            //if we are the host, set the player on the first position of the rounds spawn positions
            if (PhotonNetwork.IsMasterClient)
            {
                myPlayer.transform.position = round.player1Pos.position;
            }
            //if not, set the player on the second position of the rounds spawn positions
            else
            {
                myPlayer.transform.position = round.player2Pos.position;
            }

            //waiting a second
            await Task.Delay(1000);

            //countdown from a number

            for (int j = 3 /* starting number*/ ; j > 0; j--)
            {
                countdown.text = j.ToString();
                await Task.Delay(1000);
            }
            //after countdown, players can move and fight
            Server.SetMovementActive(true);

            countdown.text = "GO!";

            //check and wait if a player has lost
            await Task.WhenAll(WaitForPlayerDead());

            //Round celebration for winner
            Player winner = FindWinner();

            
            Server.SetMovementActive(false);

            await Task.WhenAll(CelebrateRoundWon(winner));

            //restart the cycle until all rounds have been played;
            //reset player properties to default
            Server.ResetPlayersProperties();
        }

        //end celebration for the winner
    }

    public async Task WaitForPlayerDead()
    {
        bool playerDead = false;
        
        do {
            var MyPlayerProperties = Server.MyPlayer.CustomProperties;
            var OtherPlayerProperties = Server.OtherPlayer.CustomProperties;

            if((float)MyPlayerProperties[Server.kHealth] <= 0)
            {
                playerDead = true;
            }
            if ((float)OtherPlayerProperties[Server.kHealth] <= 0)
            {
                playerDead = true;
            }

            await Task.Yield();

        } while (playerDead == false);

    }

    public async Task CelebrateRoundWon(Player wonPlayer)
    {
        if (wonPlayer == Server.MyPlayer)// The player who won will spawn and controll the celebration
        {
            var spotLight = PhotonNetwork.Instantiate(roundWonSpotlight.name, (Vector3)wonPlayer.CustomProperties[Server.kPlayerPosition], Quaternion.identity);
            float timer = 5;

            var wonPlayerProperties = wonPlayer.CustomProperties;

            var count = (float)wonPlayerProperties[Server.kRoundsWon];
            count += 1;

            wonPlayerProperties[Server.kRoundsWon] = count;

            wonPlayer.SetCustomProperties(wonPlayerProperties);

            while (timer >= 0) 
            {
                Vector3 position = (Vector3)wonPlayer.CustomProperties[Server.kPlayerPosition];
                spotLight.transform.position = position + Vector3.up * 10;
                await Task.Yield();
                timer -= Time.deltaTime;
            }

            PhotonNetwork.Destroy(spotLight);
        }
    }

    public Player FindWinner()
    {
        var MyPlayerProperties = Server.MyPlayer.CustomProperties;
        var OtherPlayerProperties = Server.OtherPlayer.CustomProperties;

        if ((float)MyPlayerProperties[Server.kHealth] <= 0)
        {
            return Server.MyPlayer;
        }

        return Server.OtherPlayer;
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
