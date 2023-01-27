using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class RoomCreator : MonoBehaviour
{
    public TMP_InputField roomNameField;
    public TMP_InputField playerNickname;
    public TextMeshProUGUI mapIndexText;
    public Image currentLevelImage;

    [Range(1,2)]
    public int mapIndex;
    public Sprite[] mapSprites;

    public Hastable currentProperties;

    public void TryCreate()
    {
        if (!roomNameField.text.IsNullOrEmpty())
        {
            string name = mapIndex.ToString() +" " + roomNameField.text;

            currentProperties = GetCurrentProperties();
            if (GameManager.MainMenu.TryCreateRoom(name, currentProperties))
            {
                Debug.Log("Room Succesfully Created!");
            }
        }
    }

    public Hastable GetCurrentProperties()
    {
        Hastable customProperties = new Hastable();
        customProperties.Add(ServerData.mapIndexProperty, mapIndex);
        return customProperties;
    }
    private void OnEnable()
    {
        roomNameField.text = string.Empty;
    }

    public void ScrollThroughLevel(int amount)
    {
        mapIndex += amount;
        if (mapIndex < 1) mapIndex = 2;
        else if (mapIndex > 2) mapIndex = 1;

        mapIndexText.text = mapIndex.ToString();
        currentLevelImage.sprite = mapSprites[mapIndex - 1];
    }
}
