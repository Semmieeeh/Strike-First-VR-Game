using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainMenuManager : MonoBehaviour
{
    [Tooltip("All The animators that get triggered whenever the play button is pressed on the start screen")]
    public AnimatorData[] gameStartAnimators;

    [Header("Menu Pages")]
    public MenuPage[] menuPages;

    public float pageSwitchWaitTime;

    [Header("Credits Settings")]
    [Header("Enter")]
    public AnimatorData[] creditsDisableAnimators;

    public AnimatorData[] creditsEnableAnimators;
    [Header("Exit")]
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

    /// <summary>
    /// Function called whenever the player pressed the Credits button in the main menu
    /// </summary>
    public async void OnCreditsEnter()
    {
        for (int i = 0; i < creditsDisableAnimators.Length; i++)
        {
            creditsDisableAnimators[i].Start();
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
