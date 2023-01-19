using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using WebSocketSharp;

public class ServerData : MonoBehaviourPunCallbacks
{
    [Header("Data")]
    public string roomName;
    public int mapIndex;
    public Sprite levelSprite;
    public bool isServerFull;

    [Header("Data References")]
    public TextMeshProUGUI mapNameText;
    public Image mapImage;

    [Header("Visual References")]
    public Image border;
    public TextMeshProUGUI JoinText;
    public Color fullColor, notFullColor;

    public TMP_InputField nickname;
    public const string mapIndexProperty = "M";

    public void Initialize(string roomName, Sprite levelSprite, int mapIndex)
    {
        this.roomName = roomName;
        this.levelSprite = levelSprite;
        this.mapIndex = mapIndex;

        mapNameText.text = roomName;
        mapImage.sprite = levelSprite;

        nickname = GameObject.Find("Player Name Input").GetComponent<TMP_InputField>();
    }


    public void JoinServer()
    {
        if (isServerFull) return;
        //if (nickname.text.IsNullOrEmpty()) return;

        GameManager.MainMenu.PlayerJoinedRoom(this);
    }

    public void SetServerColor(bool full)
    {
        isServerFull = full;
        Color currentColor = full ? fullColor : notFullColor;

        border.color = currentColor;
        mapNameText.color = currentColor;
        JoinText.color = currentColor;
        JoinText.text = full ? "" : "JOIN SERVER";
    }

}
