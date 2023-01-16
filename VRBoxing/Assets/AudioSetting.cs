using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetting : MonoBehaviour
{
    public float volume;
    public void SetAudio(float sliderValue)
    {
        float log = Mathf.Log10(sliderValue) * 20;

        float volume = Mathf.Pow(10, log / 20);
        AudioListener.volume = volume;
        this.volume = AudioListener.volume;
    }
}
