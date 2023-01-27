using UnityEngine.Audio;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    public int currentlyPlaying;
    public bool curPlaying;
    public bool stop;


    public void Update()
    {
        if(stop == true)
        {
            StopCoroutine(CountDown(12));
        }
    }

    public void PlayAudio(int toPLay, float minimalPitch, float maximumPitch)
    {
        PlayAudio(toPLay, minimalPitch, maximumPitch, audioSources[toPLay].volume);
    }
    public void PlayAudio(int toPLay, float minimalPitch, float maximumPitch, float volume)
    {

        float newPitch = Random.Range(minimalPitch, maximumPitch);
        audioSources[toPLay].pitch = newPitch;
        audioSources[toPLay].volume = volume;
        audioSources[toPLay].Play();
        currentlyPlaying = toPLay;
    }

    public void PlayArmAudio(int toPLay, float pitch, float volume)
    {

        audioSources[toPLay].volume = volume;
        audioSources[toPLay].pitch = pitch;
        audioSources[toPLay].Play();
        
        currentlyPlaying = toPLay;
        
    }
    public void SetPitch(int toPlay, float volume, float pitch)
    {
        
        audioSources[toPlay].volume = volume;
        audioSources[toPlay].pitch = pitch;
        
    }
    public bool IsPlaying(int toCheck)
    {
        return audioSources[toCheck].isPlaying; 
    }

    public void StopAudio(int toStop)
    {
        audioSources[toStop].Stop();

    }
    public void StopAllAudio()
    {
        for(int i =0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
        }
        

    }
    public void PlayOnce(int toPlay)
    {
        if(curPlaying == true)
        {
            CountDown(toPlay);
        }

    }
    
    public IEnumerator CountDown(int toPlay)
    {
        
        audioSources[12].Play();
        yield return new WaitForSeconds(17.943f);
        StopAudio(toPlay);
        
    }

    
}
