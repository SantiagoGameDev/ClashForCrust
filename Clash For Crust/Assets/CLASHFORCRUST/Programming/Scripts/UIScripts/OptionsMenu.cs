using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private static OptionsMenu instance;
    public static OptionsMenu Instance { get { return instance; } }

    [SerializeField] Slider masterVSlider;
    [SerializeField] Slider musicVSlider;
    [SerializeField] Slider sfxVSlider;
    [SerializeField] Slider announcerVSlider;

    public AudioMixer mixer;

    [SerializeField] MainMenuUI uiMenu;

    [SerializeField] Button optionsBtn;
    void Start()
    {
        if (!instance)
            instance = this;
    }

    public void OnMasterChange(float sliderValue)
    {
        mixer.SetFloat("MasterVol", sliderValue);
    }
    public void OnMusicChange(float sliderValue)
    {
        mixer.SetFloat("MusicVol", sliderValue);
    }
    public void OnSfxChange(float sliderValue)
    {
        mixer.SetFloat("SFXVol", sliderValue);
    }
    public void OnAnnouncerChange(float sliderValue)
    {
        mixer.SetFloat("AnnouncerVol", sliderValue);
    }
    public void OnExitPress()
    {
        uiMenu.ChangeButtonStatus();
        optionsBtn.Select();
        this.gameObject.SetActive(false);

    }
    public void SelectFirst()
    {
        masterVSlider.Select();
    }
}
