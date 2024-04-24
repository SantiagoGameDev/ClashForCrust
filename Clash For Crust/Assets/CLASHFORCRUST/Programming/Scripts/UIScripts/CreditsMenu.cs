using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    private static CreditsMenu instance;
    public static CreditsMenu Instance { get { return instance; } }

    [SerializeField] Button creditsBtn;
    [SerializeField] MainMenuUI mainMenuUI;

    private void Awake()
    {
        instance = this;
    }

    public void OnBPress()
    {
        mainMenuUI.ChangeButtonStatus();
        creditsBtn.Select();
        gameObject.SetActive(false);
    }
}
