using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection;
using Photon.Pun;
using TMPro;
using WebSocketSharp;
using Hastable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public const int inspectorSpace = 8;

    [Tooltip("All The animators that get triggered whenever the play button is pressed on the start screen")]
    public AnimatorData[] gameStartAnimators;

    [Space(inspectorSpace)]
    [Tooltip("All the animators that will get triggered whenever the player goes to a submenu")]
    public AnimatorData[] baseDisableAnimators;

    [Space(inspectorSpace)]
    [Header("Menu Pages")]
    public MenuPage[] menuPages;

    public float pageSwitchWaitTime;

    [Space(inspectorSpace)]
    [Header("Start Page Settings")]
    public TextMeshProUGUI connectedText;
    public GameObject servers;
    public AnimatorData[] connectedAnimators, disconnectedAnimators;

    [Space(inspectorSpace)]
    public AnimatorData[] startEnableAnimators, startDisableAnimators;

    [Space(inspectorSpace)]
    public AnimatorData createLobbySettingsAnimator;
    public AnimatorData createLobbySettingsAnimatorExit;

    [Header("Options Settings")]
    public Animator[] lineAnimators;
    public float lineToggleTime;

    [Space(inspectorSpace)]
    public AnimatorData[] optionsEnableAnimators;
    public AnimatorData[] optionsDisableAnimators;

    [Space(inspectorSpace)]
    public Animator optionsTabAnimator;
    public GameObject settings;

    [Space(inspectorSpace)]
    [Header("Wardrobe Settings")]
    public AnimatorData[] wardrobeEnableAnimators;
    public AnimatorData[] wardrobeDisableAnimators;
 
    [Space(inspectorSpace)]
    [Header("Credits Settings")]
    public AnimatorData[] creditsEnableAnimators;

    public AnimatorData[] creditsExitAnimators;

    [Header("Other")]
    public RoomCreator roomCreator;
    public ConnectToServer serverConnector;
    public PlayerMaterialManager materialManager;
    public TMP_InputField nickNameInput;
    public Button startButton;
    private AudioSource audio;
    private float minVolume = 0.1f;
    private bool canFade;

    public void Start()
    {
        audio = GameObject.Find("Main Menu Music").GetComponent<AudioSource>();
        OnGameStart();
    }
    private void Update()
    {
        startButton.interactable = !nickNameInput.text.IsNullOrEmpty();
        if(canFade == true)
        {

            Fadeout();
            if(audio.volume < minVolume)
            {
                audio.volume = minVolume;
                canFade = false;
            }
        }
    }
    /// <summary>
    /// Function called whenever the player pressed the Play button on the start screen
    /// </summary>
    public void OnGameStart()
    {
        if(!GameManager.HasUsername)
        {
            if (nickNameInput.text.IsNullOrEmpty()) return;

            GameManager.LocalPlayerNickName = nickNameInput.text;
            GameManager.HasUsername = true;
        }


        OpenPage("Main Screen",true);
        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }
        canFade = true;
    }

    public void Fadeout()
    {
        audio.volume -= 0.2f * Time.deltaTime;
    }
    #region Start Game

    public async void OnEnterStart()
    {
        for (int i = 0; i < baseDisableAnimators.Length; i++)
        {
            baseDisableAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime));

        OpenPage("Start",true);
        servers.SetActive(true);
        connectedText.text = "Connecting to server...";
    }

    public bool createGameActive;

    public void CreateGameActive(bool value) => createGameActive = value;
    public void OnServerBackButton()
    {
        if (createGameActive)
        {
            BackToServerList();
            createGameActive = false;
        }
        else
        {
            serverConnector.Disconnect();
            Disconnect();
        }
    }

    public void ToggleCreateLobbySettings()
    {
        createLobbySettingsAnimator.animator.gameObject.SetActive(true);

        createLobbySettingsAnimator.Start();
    }

    public async void OnExitStart()
    {
        connectedText.text = "Disconnected";

        for (int i = 0; i < startDisableAnimators.Length; i++)
        {
            startDisableAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime));

        OpenPage("Main Screen", true);

        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }
    }

    /// <summary>
    /// Callback function whenever the player connected to the server
    /// </summary>
    public async override void OnJoinedLobby()
    {
        for (int i = 0; i < connectedAnimators.Length; i++)
        {
            connectedAnimators[i].Start();
        }
        await Task.Delay(ToMilliseconds(0.25f));

        connectedText.text = "Connected";
    }

    public void BackToServerList()
    {
        createLobbySettingsAnimatorExit.Start();
        servers.SetActive(true);
    }

    public void Disconnect()
    {
        for (int i = 0; i < disconnectedAnimators.Length; i++)
        {
            disconnectedAnimators[i].Start();
        }

        connectedText.text = "Disconnecting...";
    }

    public void PlayerJoinedRoom(ServerData server)
    {
        connectedText.text = "Joining..." + server.roomName;
        PhotonNetwork.JoinRoom(server.roomName);
    }

    public override void OnJoinedRoom()
    {
        if(createdRoom)
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        
        Player player = PhotonNetwork.LocalPlayer;

        print(player.NickName + "1");
        if(player.NickName.IsNullOrEmpty())
            player.NickName = GameManager.LocalPlayerNickName;
        print(player.NickName + "2");
        var props = player.CustomProperties;
        props.Clear();
        props.Add(Server.kDamageLevel, 0);
        props.Add(Server.kSkinColor, materialManager.skinColorIndex);
        props.Add(Server.kHairCut, materialManager.hairCutIndex);
        props.Add(Server.kGlovesColor, materialManager.glovesColorIndex);
        props.Add(Server.kHairCutColor, materialManager.hairCutColorIndex);
        props.Add(Server.kShortsColor, materialManager.shortsColorIndex);

        player.SetCustomProperties(props);

        PhotonNetwork.LoadLevel(GetCurrentServerMapIndex());
    }

    public int mapSceneIndexOffset;
    int GetCurrentServerMapIndex()
    {
        return int.Parse(PhotonNetwork.CurrentRoom.Name[0].ToString());
    }

    public bool createdRoom;
    public Hastable properties;
    public bool TryCreateRoom(string roomName, Hastable properties)
    {
        bool succeeded = false;

        RoomOptions options = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2,
        };

        options.CustomRoomProperties = properties;
        succeeded = PhotonNetwork.CreateRoom(roomName, options);

        print(properties.ToString());
        print("Room " + roomName + " created succesfully!");

        createdRoom = true;
        this.properties = properties;
        return succeeded;
    }
    #endregion

    #region Credits
    /// <summary>
    /// Function called whenever the player pressed the Credits button in the main menu
    /// </summary>
    public async void OnCreditsEnter()
    {
        for (int i = 0; i < baseDisableAnimators.Length; i++)
        {
            baseDisableAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime));
        
        OpenPage("Credits", true);
        for (int i = 0; i < creditsEnableAnimators.Length; i++)
        {
            creditsEnableAnimators[i].Start();
        }
    }

    public async void OnCreditsExit()
    {
        for (int i = 0; i < creditsExitAnimators.Length; i++)
        {
            creditsExitAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime - 0.25f));
        
        OpenPage("Main Screen",true);

        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }

    }
    #endregion

    #region Wardrobe
    /// <summary>
    /// Function called whenever the player presses the wardrobe button in the main menu
    /// </summary>
    public async void OnWardrobeEnter()
    {
        for (int i = 0; i < baseDisableAnimators.Length; i++)
        {
            baseDisableAnimators[i].Start();
        }
        await Task.Delay(ToMilliseconds(pageSwitchWaitTime));

        OpenPage("Wardrobe", true);

        for (int i = 0; i < wardrobeEnableAnimators.Length; i++)
        {
            wardrobeEnableAnimators[i].Start();
        }
    }
    /// <summary>
    /// Function called whenever the player presses the back button in the wardrobe menu
    /// </summary>
    public async void OnWardrobeExit()
    {
        for (int i = 0; i < wardrobeDisableAnimators.Length; i++)
        {
            wardrobeDisableAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime + lineToggleTime * lineAnimators.Length));

        OpenPage("Main Screen", true);

        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }
    }

    #endregion

    #region Options

    /// <summary>
    /// Function called whenever the player pressed the Options button in the main menu
    /// </summary>
    public async void OnOptionEnter()
    {
        for (int i = 0; i < baseDisableAnimators.Length; i++)
        {
            baseDisableAnimators[i].Start();
        }

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime));

        OpenPage("Options",true);
        optionsTabAnimator.SetBool("Toggle", true);
        ToggleOptionsLine(true);

        for (int i = 0; i < optionsEnableAnimators.Length; i++)
        {
            optionsEnableAnimators[i].Start();
        }

    }

    /// <summary>
    /// Function called whenever the player pressed the Back button in the Options menu
    /// </summary>
    public async void OnOptionsExit()
    {
        for (int i = 0; i < optionsDisableAnimators.Length; i++)
        {
            optionsDisableAnimators[i].Start();
        }

        optionsTabAnimator.SetBool("Toggle", false);
        ToggleOptionsLine(false);

        await Task.Delay(ToMilliseconds(pageSwitchWaitTime + lineToggleTime * lineAnimators.Length));

        OpenPage("Main Screen", true);

        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }

    }

    /// <summary>
    /// Toggles the Animation of the line in the Options menu
    /// </summary>
    /// <param name="enable"></param>
    public async void ToggleOptionsLine(bool enable)
    {
        foreach (var animator in lineAnimators)
        {
            animator.SetBool("Enabled", enable);
            await Task.Delay(ToMilliseconds(lineToggleTime));
        }
    }

    #endregion 

    void ClosePage() => OpenPage("",true);
    
    /// <summary>
    /// Activates a SubMenu named pageName
    /// </summary>
    /// <param name="pageName"></param>
    public void OpenPage(string pageName, bool closeOtherPages)
    {
        for (int i = 0; i < menuPages.Length; i++)
        {
            if (menuPages[i].pageName == pageName)
            {
                if (menuPages[i].gameObject != null)
                {
                    menuPages[i].gameObject.SetActive(true);
                    print($"Page "+ pageName + " has opened!");
                }
                continue;
            }
            if (menuPages[i].gameObject != null && closeOtherPages)
                menuPages[i].gameObject.SetActive(false);
        }
    }

    public int ToMilliseconds(float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }

    [System.Serializable]
    public struct MenuPage
    {
        public GameObject gameObject;
        public string pageName;
    }

    public enum OptionsMenu
    {
        Video,
        Gameplay,
        Audio
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MainMenuManager))]
[CanEditMultipleObjects]
public class MainMenuEditor : Editor
{
    MainMenuManager manager;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        manager = target as MainMenuManager;

        if(Application.isPlaying && GUILayout.Button("Test Start"))
        {
            manager.OnGameStart();
        }
    }
}
#endif
