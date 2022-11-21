using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] MainMenuManager mainMenuManager;

    public static MainMenuManager MainMenu
    {
        get
        {
            return instance.mainMenuManager;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
}
