using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsManager : MonoBehaviour
{
    private static CreditsManager instance;
    public static CreditsManager Instance { get { return instance; } }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] List<AudioClip> names = new List<AudioClip>();
    [SerializeField] AudioClip cfc, kane;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void OnCancelPress(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            ReturnToMenu();
    }

    public void StartCreditsAudio()
    {
        StartCoroutine(CreditsAudio());
    }

    //public void PlayNameAudio(int i)
    //{
    //    switch (i)
    //    {
    //        case 0: audioSource.clip = santi; break;
    //        case 1: audioSource.clip = jr; break;
    //        case 2: audioSource.clip = branson; break;
    //        case 3: audioSource.clip = drew; break;
    //        case 4: audioSource.clip = dan; break;
    //        case 5: audioSource.clip = vanessa; break;
    //        case 6: audioSource.clip = nick; break;
    //        case 7: audioSource.clip = connor; break;
    //        case 8: audioSource.clip = reese; break;
    //        case 9: audioSource.clip = kane; break;

    //    }

    //    audioSource.Play();
    //}

    private IEnumerator CreditsAudio()
    {
        foreach (AudioClip clip in names)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    public void PlaySpecific(int i)
    {
        if (i == 0)
            audioSource.clip = cfc;
        else if(i == 1)
            audioSource.clip = kane;

        audioSource.Play();
    }

    public void ReturnToMenu()
    {
        SceneControl.Instance.LoadAScene(0, "Menu");
    }
}
