using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class RoomCreator : MonoBehaviour
{
    public TMP_InputField roomNameField;

    public void TryCreate()
    {
        if (!roomNameField.text.IsNullOrEmpty())
        {
            GameManager.MainMenu.TryCreateRoom(roomNameField.text);
        }
    }
}
