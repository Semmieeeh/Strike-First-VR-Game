using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using JetBrains.Annotations;

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

    public TextMeshProUGUI countdown, countdownTimer, roundText;

    public float currentRoundTimer;
    int currentRound;

    [Header("Disconnected")]
    public GameObject disconnectedObject;


    [Header("Round Celebration")]
    public GameObject roundWonSpotlight;
    public ParticleSystem[] celebrationEffects;
    public AudioSource cheer, bells;

    Room currentRoom;

    public bool LobbyFull
    {
        get
        {
            currentRoom = PhotonNetwork.CurrentRoom;
            return currentRoom.PlayerCount >= currentRoom.MaxPlayers;
        }
    }

    bool gameStarted;
    bool prepareGameStarted;
    bool playerDisconnected;
    public void Update()
    {
        currentRoom = PhotonNetwork.CurrentRoom;

        // Only the client of the host will update the in-game display. the other client(s) will fetch the data of the in-game display
        if (PhotonNetwork.IsMasterClient)
        {   
            UpdateCanvas();
        }

        if(currentRoundTimer > 0 && roundActive)
        {
            currentRoundTimer -= Time.deltaTime;
            photonView.RPC(nameof(SetTimerText), RpcTarget.All, currentRoundTimer);
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
            photonView.RPC(nameof(UpdateInGameStats), RpcTarget.All, currentRound);
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

    bool roundActive;
    async void StartGame()
    {
        print("player vinden");
        var myPlayer = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < rounds.Length; i++)
        {
            if(i == 0)
            {
                photonView.RPC(nameof(PlayAudio), RpcTarget.All);
            }
            if (playerDisconnected) return;
            var round = rounds[i];
            currentRoundTimer = round.time;

            currentRound = i;
            print("Rounds");

            photonView.RPC(nameof(ResetPlayerProperties), RpcTarget.All);
            print("Properties have been reset");

            Server.ResetHealth();
            //set player on the right spot
            //Syncronizes the positin of the players to the right spot via RPC
            photonView.RPC(nameof(SetPlayerToPosition),RpcTarget.All, currentRound);

            Server.SetMovementActive(false);
            //waiting a second
            print("Waiting a second");
            await Task.Delay(1000);

            if (playerDisconnected) return;

            print("succesfully waited a second");
            //countdown from a number

            for (int j = 3 /* starting number*/ ; j > 0; j--)
            {
                if (playerDisconnected) return;
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

            if (playerDisconnected) return;

            //check and wait if a player has lost
            await Task.WhenAll(WaitForRoundOver());

            if (playerDisconnected) return;
            print("Round Over!");

            //Round celebration for winner
            Player winner = FindWinner();
            print(winner.NickName + " Has Won!");

            //Syncronizes the winner of the round across the network
            photonView.RPC(nameof(SetWinnerOfRound), RpcTarget.All, winner.NickName, i);

            photonView.RPC(nameof(CelebrateRoundOver),RpcTarget.All);

            //Synchronizes the status of the round across the network
            photonView.RPC(nameof(SetRoundOver), RpcTarget.All, i);

            print("Started Celebration");

            //await Task.WhenAll(CelebrateRoundWon(winner));
            photonView.RPC(nameof(StartCelebration), RpcTarget.All);

            if (playerDisconnected) return;
            await Task.Delay(5000);
            print("Stopped celebration!");

            //restart the cycle until all rounds have been played;
            //reset player properties to default
            photonView.RPC(nameof(ResetPlayerProperties), RpcTarget.All);
            print("Properties have been reset");
        }

        Server.Winner = FindGameWinner();
        //end celebration for the winner
        photonView.RPC(nameof(LoadEndGameScene), RpcTarget.All);
    }

    [PunRPC]
    public void PlayAudio()
    {
        GameObject.Find("AudioShit").GetComponent<AudioManager>().PlayAudio(8, 1, 1);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        photonView.RPC(nameof(PlayerDisconnected),RpcTarget.All);
    }

    [PunRPC]
    public async void PlayerDisconnected()
    {
        //turn of main ui

        preGameObject.SetActive(false);
        inGameObject.SetActive(false);
        //enable disconnect ui

        disconnectedObject.SetActive(true);

        //leave game and return to main menu
        await Task.Delay(5000);

        PhotonNetwork.LeaveRoom();
    }

    

    [PunRPC]
    public void ResetPlayerProperties()
    {
        Server.ResetPlayersProperties();
    }

    [PunRPC]
    public void SetTimerText(float time)
    {
        countdownTimer.text = Mathf.RoundToInt(time).ToString();
    }
    
    public async Task WaitForRoundOver()
    {
        bool gameOver = false;
        
        do {
            if (playerDisconnected) return;
            var MyPlayerProperties = Server.MyPlayer.CustomProperties;
            var OtherPlayerProperties = Server.OtherPlayer.CustomProperties;

            if((float)MyPlayerProperties[Server.kHealth] <= 0f)
            {
                gameOver = true;
            }
            if ((float)OtherPlayerProperties[Server.kHealth] <= 0f)
            {
                gameOver = true;
            }
            if(currentRoundTimer <= 0)
            {
                gameOver = true;
            }

            await Task.Yield();

        } while (gameOver == false);

        print("Round Over 2!");

    }

    [PunRPC]
    public void StartCelebration()
    {
        
        if (PhotonNetwork.IsMasterClient)
            CelebrateRoundWon(FindWinner());
    }
    public void CelebrateRoundWon(Player wonPlayer)
    {
        var wonPlayerProperties = wonPlayer.CustomProperties;

        int count = (int)wonPlayerProperties[Server.kRoundsWon];
        count += 1;

        wonPlayerProperties[Server.kRoundsWon] = count;

        wonPlayer.SetCustomProperties(wonPlayerProperties);
    }

    public Player FindWinner()
    {
        Player winner = null;
        float highestHealth = 0;

        if ((float)Server.MyPlayer.CustomProperties[Server.kHealth] > highestHealth)
        {
            winner = Server.MyPlayer;
            highestHealth = (float)Server.MyPlayer.CustomProperties[Server.kHealth];
        }
        if ((float)Server.OtherPlayer.CustomProperties[Server.kHealth] > highestHealth)
        {
            winner = Server.OtherPlayer;
            highestHealth = (float)Server.OtherPlayer.CustomProperties[Server.kHealth];
        }

        return winner;
    }

    public Player FindGameWinner()
    {
        Player winner = null;
        int highestRoundWon = 0;

        if ((int)Server.MyPlayer.CustomProperties[Server.kRoundsWon] > highestRoundWon)
        {
            winner = Server.MyPlayer;
            highestRoundWon = (int)Server.MyPlayer.CustomProperties[Server.kRoundsWon];
        }
        if ((int)Server.OtherPlayer.CustomProperties[Server.kRoundsWon] > highestRoundWon)
        {
            winner = Server.OtherPlayer;
            highestRoundWon = (int)Server.OtherPlayer.CustomProperties[Server.kRoundsWon];
        }

        return winner;
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
    void UpdateInGameStats(int round)
    {
        print("In Game Stats geupdated");
        prepareGameStarted = true;
        preGameObject.SetActive(false);
        inGameObject.SetActive(true);

        //In Game
        roundText.text = $"Round {round + 1}";
        //Update Player 1 settings
        var player1Properties = Server.MyPlayer.CustomProperties;
        player1InGame.text = Server.MyPlayer.NickName;
        player1Health.text = (Mathf.RoundToInt((float)player1Properties[Server.kHealth])).ToString() + "%";
        player1RoundsWon.text = ((int)player1Properties[Server.kRoundsWon]).ToString() + " Rounds Won";

        //Update Player 2 settings
        var player2Properties = Server.OtherPlayer.CustomProperties;
        player2InGame.text = Server.OtherPlayer.NickName;
        player2Health.text = (Mathf.RoundToInt((float)player2Properties[Server.kHealth])).ToString() + "%";
        player2RoundsWon.text = ((int)player2Properties[Server.kRoundsWon]).ToString() + "Rounds Won";

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
        cheer.Play();
        bells.Play();
    }
    public void BellAudio()
    {
        AudioManager audio = GameObject.Find("AudioShit").GetComponent<AudioManager>();
        audio.PlayAudio(0, 1, 1);
    }
    [PunRPC]
    public void SetPlayerToPosition(int roundIndex)
    {
        photonView.RPC(nameof(BellAudio), RpcTarget.All);
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

    [PunRPC]
    public void LoadEndGameScene()
    {
        PhotonNetwork.LoadLevel(3);
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
