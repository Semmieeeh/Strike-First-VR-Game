using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection;
using Photon.Pun;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [Tooltip("All The animators that get triggered whenever the play button is pressed on the start screen")]
    public AnimatorData[] gameStartAnimators;

    [Tooltip("All the animators that will get triggered whenever the player goes to a submenu")]
    public AnimatorData[] baseDisableAnimators;

    [Header("Menu Pages")]
    public MenuPage[] menuPages;

    public float pageSwitchWaitTime;

    [Header("Start Page Settings")]
    public TextMeshProUGUI connectedText;
    public AnimatorData connectedAnimator, createAnimator;

    [Header("Options Settings")]
    public Animator[] lineAnimators;
    public float lineToggleTime;

    public AnimatorData[] optionsEnableAnimators;
    public AnimatorData[] optionsDisableAnimators;

    public Animator optionsTabAnimator;
    public GameObject video, gameplay, _audio;

    [Header("Credits Settings")]


    public AnimatorData[] creditsEnableAnimators;

    public AnimatorData[] creditsExitAnimators;

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
    }
    public async override void OnJoinedLobby()
    {
        connectedAnimator.Start();
        await Task.Delay(ToMilliseconds(0.25f));

        connectedText.text = "Connected";
        createAnimator.animator.gameObject.SetActive(true);
        createAnimator.Start();
    }

    public void PlayerJoinedRoom(ServerData server)
    {
        connectedText.text = "Joining..." + server.roomName;
    }

    public void TryCreateRoom(string roomName)
    {
        print("Room " + roomName + " created succesfully!");
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
        
        OpenPage("Credits", false);
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
