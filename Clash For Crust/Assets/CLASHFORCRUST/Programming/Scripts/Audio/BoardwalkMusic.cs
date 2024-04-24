using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BoardwalkMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip boardwalkMusic;
    
    [SerializeField] private AudioMixerGroup mixerGroup;
    
    private void Awake()
    {
        PlayBoardwalkMusic();
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    private void PlayBoardwalkMusic()
    {
        audioSource.clip = boardwalkMusic;
        audioSource.Play();
    }
}
