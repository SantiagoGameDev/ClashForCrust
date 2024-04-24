//For some reason the only way I could get this to work was to make it its own script tee hee xD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnnouncer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PirateModeToggle(false);
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.TitleScreen, true);
    }
}
