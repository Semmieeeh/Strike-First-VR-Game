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

    public TMP_InputField target;

    StringBuilder sb = new StringBuilder();

    public float scaleSmoothSpeed;
    Vector3 scaleVelocity;

    public Vector3 targetScale;
    public Vector3 defaultScale;

    private void Start()
    {
        defaultScale = transform.localScale;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void ToggleOn()
    {
        gameObject.SetActive(true);
        targetScale = defaultScale;
    }
    public void ToggleOff()
    {
        gameObject.SetActive(false);
        targetScale = Vector3.zero;
    }

    private void Update()
    {
        currentText.text = current;
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleVelocity, scaleSmoothSpeed);
    }
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

    public void Apply()
    {
        target.text = current;
        current = null;
        ToggleOff();
    }
}
