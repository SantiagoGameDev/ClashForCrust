using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class MenuMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip music1, music2, music3, music4, altMusic1, altMusic2, altMusic3, altMusic4;

    [SerializeField] private AudioMixerGroup mixerGroup;

    private Scene currentScene;

    private void Awake()
    {
        PlayRandomMenuMusic();
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    private void Start()
    {
        //currentScene = SceneManager.GetActiveScene();
    }

    private void PlayRandomMenuMusic()
    {
        int songChoice = Random.Range(1, 4);

        if (songChoice == 1)
        {
            audioSource.clip = music1;
            audioSource.Play();
        }
        else if (songChoice == 2)
        {
            audioSource.clip = music2;
            audioSource.Play();
        }
        else if (songChoice == 3)
        {
            audioSource.clip = music3;
            audioSource.Play();
        }
        else if (songChoice == 4)
        {
            audioSource.clip = music4;
            audioSource.Play();
        }
    }

    // private void Update()
    // {
    //     if (SceneManager.GetActiveScene().name == "Carnival" || SceneManager.GetActiveScene().name == "Boardwalk" || SceneManager.GetActiveScene().name == "PiratePeak")
    //     {
    //         if (WorldData.Instance.GetGameTime() < 0.50f)
    //         {
    //             if (audioSource.clip == music1)
    //             {
    //                 audioSource.clip = altMusic1;
    //             }
    //             else if (audioSource.clip == music2)
    //             {
    //                 audioSource.clip = altMusic2;
    //             }
    //             else if (audioSource.clip == music3)
    //             {
    //                 audioSource.clip = altMusic3;
    //             }
    //             else if (audioSource.clip == music4)
    //             {
    //                 audioSource.clip = altMusic4;
    //             }
    //         }
    //     }
    // }
}