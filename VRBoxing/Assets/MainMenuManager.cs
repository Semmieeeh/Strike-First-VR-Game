using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection;
using Photon.Pun;
using TMPro;
using Hastable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

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
    public AnimatorData[] connectedAnimators, disconnectedAnimators;

    [Space(inspectorSpace)]
    public AnimatorData[] startEnableAnimators, startDisableAnimators;

    [Space(inspectorSpace)]
    public AnimatorData createLobbySettingsAnimator;

    [Header("Options Settings")]
    public Animator[] lineAnimators;
    public float lineToggleTime;

    [Space(inspectorSpace)]
    public AnimatorData[] optionsEnableAnimators;
    public AnimatorData[] optionsDisableAnimators;

    [Space(inspectorSpace)]
    public Animator optionsTabAnimator;
    public GameObject video, gameplay, _audio;

    [Space(inspectorSpace)]
    [Header("Credits Settings")]
    public AnimatorData[] creditsEnableAnimators;

    public AnimatorData[] creditsExitAnimators;

    private void Start()
    {
            
    }
    /// <summary>
    /// Function called whenever the player pressed the Play button on the start screen
    /// </summary>
    public void OnGameStart()
    {
        OpenPage("Main Screen",true);
        for (int i = 0; i < gameStartAnimators.Length; i++)
        {
            gameStartAnimators[i].Start();
        }
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

        connectedText.text = "Connecting to server...";
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
    }

    public bool TryCreateRoom(string roomName, int mapIndex, Sprite mapSprite)
    {
        try
        {
            RoomOptions options = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 2,
            };

            Hastable customProperties = new Hastable();
            customProperties.Add(ServerData.mapIndexProperty, mapIndex);

            options.CustomRoomProperties = customProperties;
            PhotonNetwork.CreateRoom(roomName, options);
            PhotonNetwork.JoinRoom(roomName);


            print("Room " + roomName + " created succesfully!");
            return true;
        }
        catch
        {
            return false;
        }
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

        ToggleOptionsSubMenu(1);

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

        ToggleOptionsLine(false);

        ToggleOptionsSubMenu(-1);

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

    int previousIndex;
    bool changing;
    public async void ToggleOptionsSubMenu(int menuIndex)
    {
        if (previousIndex == menuIndex || changing) return;
        changing = true;

        optionsTabAnimator.SetBool("Toggle",false);

        await Task.Delay(ToMilliseconds(0.25f));


        switch (menuIndex)
        {
            case (int)OptionsMenu.Video:
                video.SetActive(true);
                gameplay.SetActive(false);
                _audio.SetActive(false);
                break;
            case (int)OptionsMenu.Gameplay:
                video.SetActive(false);
                gameplay.SetActive(true);
                _audio.SetActive(false);
                break;
            case (int)OptionsMenu.Audio:
                video.SetActive(false);
                gameplay.SetActive(false);
                _audio.SetActive(true);
                break;
            default:
                video.SetActive(false);
                gameplay.SetActive(false);
                _audio.SetActive(false);
                break;
        }
        optionsTabAnimator.SetBool("Toggle", true);
        previousIndex = menuIndex;
        changing = false;
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
