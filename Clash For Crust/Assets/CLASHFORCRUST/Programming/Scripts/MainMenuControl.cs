using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenuControl : MonoBehaviour
{
    private void Start()
    {
        MultiplayerEventSystem mes = GetComponent<MultiplayerEventSystem>();

        mes.playerRoot = GameObject.Find("MainMenuCanvas");

        mes.firstSelectedGameObject = GameObject.Find("StartButton");
    }

    public void ExitHowToPlayMenu()
    {
        if (HowToPlay.Instance)
        {
            if(HowToPlay.Instance.gameObject.activeSelf) 
            { 
                HowToPlay.Instance.OnBPress(); 
            }
        }

        if (OptionsMenu.Instance)
        {
            if (OptionsMenu.Instance.gameObject.activeSelf) 
            {
                OptionsMenu.Instance.OnExitPress();
            }
        }

        if (CreditsMenu.Instance)
        {
            if (CreditsMenu.Instance.gameObject.activeSelf)
                CreditsMenu.Instance.OnBPress();
        }
    }

}
