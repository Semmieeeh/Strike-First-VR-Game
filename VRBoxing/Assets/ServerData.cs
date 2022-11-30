using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

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

    public const string mapIndexProperty = "M";

    public void Initialize(string roomName, Sprite levelSprite, int mapIndex)
    {
        this.roomName = roomName;
        this.levelSprite = levelSprite;
        this.mapIndex = mapIndex;

        mapNameText.text = roomName;
        mapImage.sprite = levelSprite;
    }

    public void JoinServer()
    {
        if (isServerFull) return;

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
