using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PiratePeakMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip pirateMusic;
    
    [SerializeField] private AudioMixerGroup mixerGroup;
    
    private void Awake()
    {
        PlayPirateMusic();
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    private void PlayPirateMusic()
    {
        audioSource.clip = pirateMusic;
        audioSource.Play();
    }
}
