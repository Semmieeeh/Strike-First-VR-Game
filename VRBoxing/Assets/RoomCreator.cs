using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class RoomCreator : MonoBehaviour
{
    public TMP_InputField roomNameField;
    public TextMeshProUGUI mapIndexText;
    public Image currentLevelImage;

    [Range(1,3)]
    public int mapIndex;
    public Sprite[] mapSprites;

    public void TryCreate()
    {
        if (!roomNameField.text.IsNullOrEmpty())
        {
            if (GameManager.MainMenu.TryCreateRoom(roomNameField.text, mapIndex, mapSprites[mapIndex -1]))
            {
                Debug.Log("Room Succesfully Created!");
            }
        }
    }

    public void ScrollThroughLevel(int amount)
    {
        mapIndex += amount;
        if (mapIndex < 1) mapIndex = 3;
        else if (mapIndex > 3) mapIndex = 1;

        mapIndexText.text = mapIndex.ToString();
        currentLevelImage.sprite = mapSprites[mapIndex - 1];
    }
}
