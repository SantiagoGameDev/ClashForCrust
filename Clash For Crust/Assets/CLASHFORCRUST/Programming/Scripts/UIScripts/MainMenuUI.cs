using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    public Button Start, Options, HTP, Credits, Quit;

    public Text AButtonText;

    public HowToPlay howToplayMenu;

    public OptionsMenu optionsMenu;

    public GameObject creditsPanel;

    public Button santiagoBtn;

    public void OnStartClick()
    {
        // Start Game
        // Load player customization screen
        if (!SceneControl.Instance.loading) { SceneControl.Instance.LoadAScene(2, "Customization"); }

        EventSystem.current.SetSelectedGameObject(null);
        ///SceneControl.Instance.LoadAScene(2, "Customization");
    }

    public void OnOptionClick()
    {
        // Transition to the option menu
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.OptionsButton, true);
        optionsMenu.gameObject.SetActive(true);
        optionsMenu.SelectFirst();
        ChangeButtonStatus();
    }

    public void OnHowToPlayClick()
    {
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.RulesMenu, true);
        howToplayMenu.gameObject.SetActive(true);
        howToplayMenu.HighlightFakeSlider();
    }

    public void OnCreditsClick()
    {
        SceneControl.Instance.LoadAScene(8, "Credits");
    }

    public void OnExitClick()
    {
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.ExitGame, true);

        yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void ChangeButtonStatus()
    {
        if (Start.interactable)
        {
            Start.interactable = false;
            Options.interactable = false;
            HTP.interactable = false;
            Credits.interactable = false;
            Quit.interactable = false;
        }
        else
        {
            Start.interactable= true;
            Options.interactable= true;
            HTP.interactable= true;
            Credits.interactable = true;
            Quit.interactable= true;
        }
    }
}
