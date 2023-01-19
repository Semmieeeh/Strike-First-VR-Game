using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatePreview : MonoBehaviour
{
    Slider slider;
    public Transform preview;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        preview.localEulerAngles = new(0,Mathf.Lerp(0, -360, slider.value),0);
    }
}
