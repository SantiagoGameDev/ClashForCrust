using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    private static HowToPlay instance;
    public static HowToPlay Instance { get { return instance; } }

    [SerializeField] private Image howToPlayImg;
    [SerializeField] List<Sprite> images;
    [SerializeField] Slider fakeSlider;
    [SerializeField] Button howToPlayBtn;
    [SerializeField] GameObject panel;
    [SerializeField] AudioSource buttonSound;
    int imageIndex;

    private void Start()
    {
        if (!instance)
            instance = this;

        imageIndex = 0;
        howToPlayImg.sprite = images[0];
        fakeSlider.minValue = 0f;
        fakeSlider.maxValue = images.Count - 1;
        
    }


    public void OnLeftStick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            CycleLeft();
        }
    }

    public void OnRightStick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            CycleRight();
        }
    }

    public void CycleLeft()
    {
        if(imageIndex > 0)
        {
            imageIndex--;
            howToPlayImg.sprite = images[imageIndex];
        }
    }

    public void CycleRight()
    {
        if (imageIndex < images.Count)
        {
            imageIndex++;
            howToPlayImg.sprite = images[imageIndex];
        }
    }

    public void HighlightFakeSlider()
    {
        fakeSlider.Select();
    }

    public void OnSliderChange()
    {
        buttonSound.Play();
        imageIndex = (int)fakeSlider.value;
        howToPlayImg.sprite = images[imageIndex];
        
    }

    public void OnBPress()
    {
        howToPlayImg.sprite = images[0];
        howToPlayBtn.Select();
        imageIndex = 0;
        fakeSlider.value = 0;
        this.gameObject.SetActive(false);
    }

}
