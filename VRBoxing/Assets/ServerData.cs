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

    [Header("References")]
    public TextMeshProUGUI mapIndexText;
    public Image mapImage; 

    public const string mapIndexProperty = "M";

    public void Initialize(string roomName, Sprite levelSprite, int mapIndex)
    {
        this.roomName = roomName;
        this.levelSprite = levelSprite;
        this.mapIndex = mapIndex;

        mapIndexText.text = mapIndex.ToString();
        mapImage.sprite = levelSprite;
    }

    public void JoinServer()
    {
        PhotonNetwork.JoinRoom(roomName);
        GameManager.MainMenu.PlayerJoinedRoom(this);
    }

}
