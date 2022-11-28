using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    public TextMeshProUGUI currentText;
    public string current;

    StringBuilder sb = new StringBuilder();
    public void AddKey(string key)
    {
        sb.Append(key);
        current = sb.ToString();
    }
    public void ResetInput()
    {
        sb.Clear();
        current = sb.ToString();
    }
    public void RemoveCharacter()
    {
        sb.Remove(sb.Length -1, 1);
        current = sb.ToString();
    }

    private void Update()
    {
        currentText.text = current;
    }
    public void Apply()
    {

    }
}
