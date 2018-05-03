using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ClipArray creates an array of audioclips, which can be used to create sound
[System.Serializable]
public class ClipArray
{
    public string _name;
    [HideInInspector]
    public AudioClip currentAudioClip;
    public AudioClip[] audioClips;

    public AudioSource source;
}

public class AudioScript : MonoBehaviour {

    public ClipArray[] clipArrays;

    public void PlayAudioRandom(int arraySource)
    {
        ClipArray currentClipArray = clipArrays[arraySource];
        if (!clipArrays[arraySource].source.isPlaying)
        {
            currentClipArray.currentAudioClip = currentClipArray.audioClips[Random.Range(0, currentClipArray.audioClips.Length - 1)];
            currentClipArray.source.clip = currentClipArray.currentAudioClip;
            currentClipArray.source.Play();
        }
    }

    public void PlayAudioOnce(int arraySource, bool overridePlayedAudio)
    {
        ClipArray currentClipArray = clipArrays[arraySource];
        if (!overridePlayedAudio)
        {
            if (!currentClipArray.source.isPlaying)
            {
                currentClipArray.currentAudioClip = currentClipArray.audioClips[0];
                currentClipArray.source.clip = currentClipArray.currentAudioClip;
                currentClipArray.source.Play();
            }
        }
        else
        {
            currentClipArray.currentAudioClip = currentClipArray.audioClips[0];
            currentClipArray.source.clip = currentClipArray.currentAudioClip;
            currentClipArray.source.Play();
        }
    }

    //Can be used to modify audio by speed for example (Used in wheel in this case)
    public void AudioModify(float targetValue, float volumeModifier, int arraySource)
    {
        ClipArray currentClipArray = clipArrays[arraySource];
        currentClipArray.source.volume = Mathf.Lerp(currentClipArray.source.volume, Mathf.Abs(targetValue) / volumeModifier, 0.4f);
    }
}
