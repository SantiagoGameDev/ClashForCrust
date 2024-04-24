using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buttonSound;

    private enum ButtonType { Start = 0, Options = 1, HowToPlay, Credits, Quit };

    [SerializeField] private ButtonType whichButton;
    public TMP_Text aButtonText;

    public void OnSelect(BaseEventData bsd)
    {
        audioSource.Play();
        switch (whichButton)
        {
            case ButtonType.Start:
                aButtonText.text = "Start Game";
                break;
            case ButtonType.Options:
                aButtonText.text = "Choose Options";
                break;
            case ButtonType.HowToPlay:
                aButtonText.text = "Learn the Rules";
                break;
            case ButtonType.Credits:
                aButtonText.text = "View Credits";
                break;
            case ButtonType.Quit:
                aButtonText.text = "Quit Game";
                break;
        }
    }
}
