using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdditionalInputField : MonoBehaviour
{
    public VirtualKeyboard keyboard;
    
    public void OnClick()
    {
        keyboard.SetTarget(GetComponent<TMP_InputField>());
    }
}
