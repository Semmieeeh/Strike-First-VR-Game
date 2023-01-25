using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

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
    public Image player1Trophy;

    public TextMeshProUGUI player2InGame, player2Health, player2RoundsWon;
    public Image player2Trophy;

    public TextMeshProUGUI countdown;

    int currentRound;
    [Header("Round Celebration")]
    public GameObject roundWonSpotlight;
    public ParticleSystem[] celebrationEffects;

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
            print("Prepare Start Game!");
            PrepareStartGame();
        }

        //update the players name
        photonView.RPC(nameof(SetPlayerNamesPreGame),RpcTarget.All);


        if (timeToWaitWhenGameCanStart <= 0)
        {
            photonView.RPC(nameof(UpdateInGameStats), RpcTarget.All);
        }


    }

    void PrepareStartGame()
    {
        print("Game Can Start!");
        timeToWaitWhenGameCanStart -= Time.deltaTime;
        if(timeToWaitWhenGameCanStart <= 0)
        {
            print("Game Started");
            StartGame();
        }
        print(timeToWaitWhenGameCanStart);
    }

    async void StartGame()
    {
        print("player vinden");
        var myPlayer = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < rounds.Length; i++)
        {
            var round = rounds[i];
            currentRound = i;
            print("Rounds");

            //set player on the right spot
            //Syncronizes the positin of the players to the right spot via RPC
            photonView.RPC(nameof(SetPlayerToPosition),RpcTarget.All,currentRound);

            Server.SetMovementActive(false);
            //waiting a second
            print("Waiting a second");
            await Task.Delay(1000);

            print("succesfully waited a second");
            //countdown from a number

            for (int j = 3 /* starting number*/ ; j > 0; j--)
            {
                print("Countdown: " + j);

                //Syncronizes the countdown across the network
                photonView.RPC(nameof(SetCountDownText), RpcTarget.All, j.ToString());
                await Task.Delay(1000);
            }
            //after countdown, players can move and fight
            Server.SetMovementActive(true);
            print("Can fight!");

            //Syncronizes the countdown across the network
            photonView.RPC(nameof(SetCountDownText), RpcTarget.All, "GO!");

            //check and wait if a player has lost
            await Task.WhenAll(WaitForPlayerDead());
            print("player died!");

            //Round celebration for winner
            Player winner = FindWinner();
            print(winner.NickName + " Has Won!");

            //Syncronizes the winner of the round across the network
            photonView.RPC(nameof(SetWinnerOfRound), RpcTarget.All, winner.NickName, i);

            photonView.RPC(nameof(CelebrateRoundOver),RpcTarget.All);

            //Synchronizes the status of the round across the network
            photonView.RPC(nameof(SetRoundOver), RpcTarget.All, i);

            print("Started Celebration");
            await Task.WhenAll(CelebrateRoundWon(winner));
            photonView.RPC(nameof(StartCelebration), RpcTarget.All, winner.NickName);

            print("Stopped celebration!");

            //restart the cycle until all rounds have been played;
            //reset player properties to default
            Server.ResetPlayersProperties();
            print("Properties have been reset");
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

    [PunRPC]
    public async void StartCelebration(string playerName)
    {
        print("celebration gecelebrate");
        Player wonPlayer = null;
        if (Server.MyPlayer.NickName == playerName)
            wonPlayer = Server.MyPlayer;
        else wonPlayer = Server.OtherPlayer;

        await CelebrateRoundWon(wonPlayer);
    }
    public async Task CelebrateRoundWon(Player wonPlayer)
    {
        if (wonPlayer == Server.MyPlayer) // The player who won will spawn and controll the celebration
        {
            var spotLight = PhotonNetwork.Instantiate(roundWonSpotlight.name, (Vector3)wonPlayer.CustomProperties[Server.kPlayerPosition], Quaternion.identity);

            var wonPlayerProperties = wonPlayer.CustomProperties;

            int count = (int)wonPlayerProperties[Server.kRoundsWon];
            count += 1;

            wonPlayerProperties[Server.kRoundsWon] = count;

            wonPlayer.SetCustomProperties(wonPlayerProperties);

            float timer = 5;
            while (timer >= 0) 
            {
                Vector3 position = (Vector3)wonPlayer.CustomProperties[Server.kPlayerPosition];
                spotLight.transform.position = position + Vector3.up * 10;
                await Task.Yield();
                timer -= Time.deltaTime;
            }

            PhotonNetwork.Destroy(spotLight);
        }
        else
        {
            await Task.Delay(5000);
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
        print("pre game geupdated");
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

        if (LobbyFull) waitingTMP.text = "Game Starting...";
    }

    [PunRPC]
    void UpdateInGameStats()
    {
        print("In Game Stats geupdated");
        prepareGameStarted = true;
        preGameObject.SetActive(false);
        inGameObject.SetActive(true);

        //In Game
        //Update Player 1 settings
        var player1Properties = Server.MyPlayer.CustomProperties;
        player1InGame.text = Server.MyPlayer.NickName;
        player1Health.text = ((float)player1Properties[Server.kHealth]).ToString("D") + "%";
        player1RoundsWon.text = ((float)player1Properties[Server.kHealth]).ToString("D") + " Rounds Won";

        //Update Player 2 settings
        var player2Properties = Server.OtherPlayer.CustomProperties;
        player2InGame.text = Server.OtherPlayer.NickName;
        player2Health.text = ((float)player2Properties[Server.kHealth]).ToString("D") + "%";
        player2RoundsWon.text = ((float)player2Properties[Server.kRoundsWon]).ToString("D") + "Rounds Won";

        //Update Trophy Picture
        int player1Wins = (int)player1Properties[Server.kRoundsWon];
        int player2Wins = (int)player2Properties[Server.kRoundsWon];

        if(player1Wins > player2Wins) // player 1 is winning
        {
            player1Trophy.gameObject.SetActive(true);
            player2Trophy.gameObject.SetActive(false);
        }
        else if(player1Wins < player2Wins)
        {
            player2Trophy.gameObject.SetActive(true);
            player1Trophy.gameObject.SetActive(false);
        }
        else
        {
            player2Trophy.gameObject.SetActive(false);
            player1Trophy.gameObject.SetActive(false);
        }
        
    }

    [PunRPC]
    public void CelebrateRoundOver()
    {
        print("Celebrate particels!");
        for (int i = 0; i < celebrationEffects.Length; i++)
        {
            celebrationEffects[i].Play();
        }
    }

    [PunRPC]
    public void SetPlayerToPosition(int roundIndex)
    {
        print("player set to position");
        var round = rounds[roundIndex];
        var myPlayer = GameObject.FindGameObjectWithTag("Player");
        if (PhotonNetwork.IsMasterClient)
        {
            myPlayer.transform.position = round.player1Pos.position;
            print("myplayer position set to pos 1");
        }
        //if not, set the player on the second position of the rounds spawn positions
        else
        {
            myPlayer.transform.position = round.player2Pos.position;
            print("myplayer position set to pos 2");
        }
    }

    [PunRPC]
    public void SetCountDownText(string text)
    {
        print("countdown text gezet");
        this.countdown.text = text;
    }

    [PunRPC]
    public void SetWinnerOfRound(string winner, int roundIndex)
    {
        print("winner of round gezet");
        rounds[roundIndex].playerThatWon = winner;
    }

    [PunRPC]
    public void SetRoundOver(int roundIndex)
    {
        print("round over gezet");
        rounds[roundIndex].roundOver = true;
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
