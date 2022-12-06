using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class PlayersManager : MonoBehaviour
{
    #region Singleton

    public static PlayersManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }
    #endregion

    /// <summary>
    /// The first player in the game, should be the owner/host of the room the player created
    /// </summary>
    public Player player1;

    /// <summary>
    /// The second player in the game
    /// </summary>
    public Player player2;

    [Header("Setup Data")]
    [SerializeField] 
    float startHealth = 100;

    [SerializeField] 
    float strengthMultiplier = 1;

    [SerializeField]
    float hitCooldown;

    [SerializeField] 
    float timePerRound;

    [SerializeField] 
    int rounds;

    [SerializeField] 
    int roundsToWin;

    #region Properties Indexing Strings

    public const string kHealth = "HE";
    public const string kRoundsWon = "RW";

    public const string kLeftHandCooldown = "LHC";
    public const string kRightHandCooldown = "RHC";

    #endregion

    //private void Start()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        player1 = 
    //    }
    //}
    public void SetUp()
    {
        if (player1 != null) SetUpPlayer(player1);
        if (player2 != null) SetUpPlayer(player2);
    }

    public void SetUpPlayer(Player player)
    {
        UpdatePlayerProperties(player, startHealth, 0, 0, 0);
    }

    /// <summary>
    /// Used to Update a Players properties. Should not be used frequently due to traffick and performance
    /// </summary>
    /// <param name="player">The targeted player whose properties will be updated</param>
    /// <param name="hp">The amount of health the player Has</param>
    /// <param name="roundsWon"> The amount of rounds that the player has won</param>
    /// <param name="leftHandCooldown">The amount of time where the player cannot punch with the left hand</param>
    /// <param name="rightHandCooldown">The amount of time where the player cannot punch with the right hand</param>
    public void UpdatePlayerProperties(Player player, float hp, int roundsWon, float leftHandCooldown, float rightHandCooldown)
    {
        var props = new Hastable();

        // Initialize or update all player properties;
        props.Add(kHealth, hp);
        props.Add(kRoundsWon, roundsWon);
        props.Add(kLeftHandCooldown, leftHandCooldown);
        props.Add(kRightHandCooldown, rightHandCooldown);

        // Used to Synchronizes the players properties to the server
        player.SetCustomProperties(props);
    }

    public void SetPlayers(Player player1, Player player2)
    {
        this.player1 = player1;
        this.player2 = player2;

        SetUp();
    }
}
