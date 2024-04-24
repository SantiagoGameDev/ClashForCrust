using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class RuleSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    enum SliderType { GameTime, PowerUps, Fries, Crust, Health, Awards, Screw }

    [SerializeField] SliderType whichSlider;
    private Slider slider;
    [SerializeField] private Image circle;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject leftArrow, rightArrow;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image image;

    private bool isSelected = false;
    private bool canChangeSlider = true;
    private float oldSliderValue;

    private float baseScale = 4.9775f;
    private bool isChangingSize = false;

    private bool goneThroughStart = false;

    private List<Color> selectedColors = new List<Color>();

    // Start is called before the first frame update
    void Start()
    {
        if (!goneThroughStart)
        {
            slider = GetComponent<Slider>();
            oldSliderValue = slider.value;

            baseScale = transform.localScale.x;

            if (AlterRulesMenu.Instance)
            {
                AlterRulesMenu.Instance.onChange += UpdateToggle;
            }

            transform.localScale = new Vector3(0.45f * baseScale, 0.45f * baseScale, 0.45f * baseScale);

            CheckArrows();
            UpdateSelectedColors();
        }

        goneThroughStart = true;
    }

    public void UpdateToggle()
    {
        //Debug.Log("Got here");

        if (isSelected)
        {
            if (toggle != null)
            {
                toggle.isOn = !toggle.isOn;
                ToggleValueChanging(toggle.isOn);

                if (toggle.isOn)
                {
                    text.enabled = false;
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(false);
                    canChangeSlider = false;
                    image.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    text.enabled = true;
                    CheckArrows();
                    canChangeSlider = true;
                    image.color = Color.white;
                }
                UpdateSelectedColors();
            }
        }
    }

    private void ToggleValueChanging(bool toggleValue)
    {
        AlterRulesMenu.Instance.rulesCC2.text = "";
        switch (whichSlider)
        {
            case SliderType.PowerUps:
                AlterRulesMenu.Instance.powerupsOn = !toggleValue;
                PowerupsText();
                break;
            case SliderType.Fries:
                AlterRulesMenu.Instance.fryOn = !toggleValue;
                FryText();
                break;
            case SliderType.Crust:
                AlterRulesMenu.Instance.crustOn = !toggleValue;
                CrustText();
                break;
            case SliderType.Awards:
                AlterRulesMenu.Instance.awardsOn = !toggleValue;
                AwardsText();
                break;
            case SliderType.Screw:
                AlterRulesMenu.Instance.screwMode = !toggleValue;
                ScrewText();
                break;
        }
    }
    public void OnSliderchange(float sliderValue)
    {
        if (canChangeSlider)
        {
            switch (whichSlider)
            {
                case SliderType.GameTime:
                    ChangeGameTime(sliderValue);
                    break;
                case SliderType.Health:
                    ChangeHealth(sliderValue);
                    break;
                case SliderType.PowerUps:
                    ChangePowerupFrequency(sliderValue);
                    break;
                case SliderType.Fries:
                    ChangeFryFrequency(sliderValue);
                    break;
                case SliderType.Crust:
                    ChangeCrustValue(sliderValue);
                    break;
                case SliderType.Awards:
                    ChangeAwardAmount(sliderValue);
                    break;
            }

            CheckArrows();

            oldSliderValue = sliderValue;
        }
        else if (sliderValue != oldSliderValue)
        {
            slider.value = oldSliderValue;
        }
    }

    public void ChangeGameTime(float sliderValue)
    {
        float gameTime = 0;
        gameTime = (sliderValue + 1) * 30;

        AlterRulesMenu.Instance.gameTime = gameTime;

        int minutes = (int)(gameTime / 60);
        float seconds = gameTime % 60;

        string sS = "";

        if (seconds == 0)
        {
            sS = "0";
        }

        text.text = minutes + ":" + sS + seconds;
        GameTimeText();
    }

    public void ChangeHealth(float sliderValue)
    {
        float health = sliderValue + 1;

        AlterRulesMenu.Instance.maxHealth = (int)health;

        text.text = health.ToString();
        HealthText();
    }

    public void ChangePowerupFrequency(float sliderValue)
    {
        float spawnRate = 0;
        string speedText = "";
        switch (sliderValue)
        {
            case 0:
                spawnRate = 10f;
                speedText = "Slw";
                break;
            case 1:
                spawnRate = 5f;
                speedText = "Avg";
                break;
            case 2:
                spawnRate = 1f;
                speedText = "Fst";
                break;
        }

        AlterRulesMenu.Instance.powerupSpawnRate = spawnRate;

        text.text = speedText;
        PowerupsText();
    }

    public void ChangeFryFrequency(float sliderValue)
    {
        float spawnRate = 0;
        string speedText = "";
        switch (sliderValue)
        {
            case 0:
                spawnRate = 10f;
                speedText = "Slw";
                break;
            case 1:
                spawnRate = 5f;
                speedText = "Avg";
                break;
            case 2:
                spawnRate = 1f;
                speedText = "Fst";
                break;
        }

        AlterRulesMenu.Instance.frySpawnRate = spawnRate;

        text.text = speedText;
        FryText();
    }

    public void ChangeCrustValue(float sliderValue)
    {
        float crustValue = sliderValue + 1;

        AlterRulesMenu.Instance.crustValue = (int)crustValue;

        text.text = crustValue.ToString();
        CrustText();
    }

    public void ChangeAwardAmount(float sliderValue)
    {
        AlterRulesMenu.Instance.numAwards = (int)sliderValue;

        text.text = sliderValue.ToString();
        AwardsText();
    }

    private void CheckArrows()
    {
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        if (slider.value == slider.minValue)
        {
            leftArrow.SetActive(false);
        }
        if (slider.value == slider.maxValue)
        {
            rightArrow.SetActive(false);
        }

        if (whichSlider == SliderType.Screw)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
    }

    public void OnSelect(BaseEventData bed)
    {
        isSelected = true;
        StartCoroutine(ChangeSize(1f));

        switch (whichSlider)
        {
            case SliderType.GameTime:
                AlterRulesMenu.Instance.selectedSlider = 0;
                AlterRulesMenu.Instance.rulesTitle.text = "How long will the Clash be?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "";
                AlterRulesMenu.Instance.AButton.SetActive(false);
                GameTimeText();
                break;
            case SliderType.Health:
                AlterRulesMenu.Instance.selectedSlider = 1;
                AlterRulesMenu.Instance.rulesTitle.text = "What will the Max Health be?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "";
                AlterRulesMenu.Instance.AButton.SetActive(false);
                HealthText();
                break;
            case SliderType.Crust:
                AlterRulesMenu.Instance.selectedSlider = 2;
                AlterRulesMenu.Instance.rulesTitle.text = "Will the Golden Crust be used?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "And how many points will it give?";
                AlterRulesMenu.Instance.AButton.SetActive(true);
                CrustText();
                break;
            case SliderType.PowerUps:
                AlterRulesMenu.Instance.selectedSlider = 3;
                AlterRulesMenu.Instance.rulesTitle.text = "Will Power-Ups be used?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "And how frequently will they spawn?";
                AlterRulesMenu.Instance.AButton.SetActive(true);
                PowerupsText();
                break;
            case SliderType.Fries:
                AlterRulesMenu.Instance.selectedSlider = 4;
                AlterRulesMenu.Instance.rulesTitle.text = "Will French Fries be used?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "And how frequently will they spawn?";
                AlterRulesMenu.Instance.AButton.SetActive(true);
                FryText();
                break;
            case SliderType.Awards:
                AlterRulesMenu.Instance.selectedSlider = 5;
                AlterRulesMenu.Instance.rulesTitle.text = "Will there be End-Game Awards?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "And how many awards will there be?";
                AlterRulesMenu.Instance.AButton.SetActive(true);
                AwardsText();
                break;
            case SliderType.Screw:
                AlterRulesMenu.Instance.selectedSlider = 6;
                AlterRulesMenu.Instance.rulesTitle.text = "Will you enable Screw Mode?";
                AlterRulesMenu.Instance.rulesSubtitle.text = "Branson's preferred way to play.";
                AlterRulesMenu.Instance.AButton.SetActive(true);
                ScrewText();
                break;

        }

        AlterRulesMenu.Instance.UpdateSliderPositions();
        StartCoroutine(AnimateSlider());
    }

    public void OnDeselect(BaseEventData bed)
    {
        isSelected = false;
        StartCoroutine(ChangeSize(0.45f));
    }

    IEnumerator ChangeSize(float scaleChange)
    {
        float startScale = transform.localScale.x / baseScale;

        float scale = 0f;

        while (isChangingSize)
        {
            yield return null;
        }

        isChangingSize = true;

        while (scale < 1f)
        {
            scale += Time.deltaTime * 7f;

            if (scale > 1f)
            {
                scale = 1f;
            }

            float scaleDif = 0;
            scaleDif = startScale - (startScale - scaleChange) * scale;

            float colorMultiplier;
            if (startScale > scaleChange)
            {
                colorMultiplier = 1f - scale * 0.5f;
            }
            else
            {
                colorMultiplier = 0.5f + scale * 0.5f;
            }

            UpdateColors(colorMultiplier);

            transform.localScale = new Vector3(scaleDif * baseScale, scaleDif * baseScale, scaleDif * baseScale);

            yield return null;
        }

        if (isSelected)
        {
            UpdateSelectedColors();
        }
        isChangingSize = false;
    }

    public void OnEnable()
    {
        Start();
    }

    public void OnDisable()
    {
        isSelected = false;
        UpdateColors(0.5f);
        transform.localScale = new Vector3(0.45f * baseScale, 0.45f * baseScale, 0.45f * baseScale);
    }

    private void GameTimeText()
    {
        string minutes;
        if (AlterRulesMenu.Instance.gameTime != 60)
        {
            minutes = " Minutes";
        }
        else
        {
            minutes = " Minute";
        }

        int minute = (int)(AlterRulesMenu.Instance.gameTime / 60);
        float seconds = AlterRulesMenu.Instance.gameTime % 60;

        string sS = "";

        if (seconds == 0)
        {
            sS = "0";
        }

        string timeLimit = minute + ":" + sS + seconds;

        AlterRulesMenu.Instance.rulesCC1.text = timeLimit + minutes;
        AlterRulesMenu.Instance.rulesCC2.text = "";
    }

    private void HealthText()
    {
        string feathers;
        if (AlterRulesMenu.Instance.maxHealth != 1)
        {
            feathers = " Feathers";
        }
        else
        {
            feathers = " Feather";
        }
        AlterRulesMenu.Instance.rulesCC1.text = AlterRulesMenu.Instance.maxHealth + feathers;
        AlterRulesMenu.Instance.rulesCC2.text = "";
    }

    private void CrustText()
    {
        if (AlterRulesMenu.Instance.crustOn)
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle Off";
            AlterRulesMenu.Instance.rulesCC1.text = "Is Being Used";
            string calories;
            if (AlterRulesMenu.Instance.crustValue != 1)
            {
                calories = " Calories";
            }
            else
            {
                calories = " Calorie";
            }
            AlterRulesMenu.Instance.rulesCC2.text = AlterRulesMenu.Instance.crustValue + calories;
        }
        else
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On";
            AlterRulesMenu.Instance.rulesCC1.text = "Is NOT Being Used";
            AlterRulesMenu.Instance.rulesCC2.text = "";
        }
    }

    private void PowerupsText()
    {
        if (AlterRulesMenu.Instance.powerupsOn)
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle Off";
            AlterRulesMenu.Instance.rulesCC1.text = "Are Being Used";
            string speed = "";
            switch (AlterRulesMenu.Instance.powerupSpawnRate)
            {
                case 1:
                    speed = "Fast (1 Second)";
                    break;
                case 5:
                    speed = "Average (5 Seconds)";
                    break;
                case 10:
                    speed = "Slow (10 Seconds)";
                    break;
            }
            AlterRulesMenu.Instance.rulesCC2.text = speed;
        }
        else
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On";
            AlterRulesMenu.Instance.rulesCC1.text = "Are NOT Being Used";
            AlterRulesMenu.Instance.rulesCC2.text = "";
        }
    }

    private void FryText()
    {
        if (AlterRulesMenu.Instance.fryOn)
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle Off";
            AlterRulesMenu.Instance.rulesCC1.text = "Are Being Used";
            string speed = "";
            switch (AlterRulesMenu.Instance.frySpawnRate)
            {
                case 1:
                    speed = "Fast (1 Second)";
                    break;
                case 5:
                    speed = "Average (5 Seconds)";
                    break;
                case 10:
                    speed = "Slow (10 Seconds)";
                    break;
            }
            AlterRulesMenu.Instance.rulesCC2.text = speed;
        }
        else
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On";
            AlterRulesMenu.Instance.rulesCC1.text = "Are NOT Being Used";
            AlterRulesMenu.Instance.rulesCC2.text = "";
        }
    }

    private void AwardsText()
    {
        if (AlterRulesMenu.Instance.awardsOn)
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle Off";
            AlterRulesMenu.Instance.rulesCC1.text = "Are Being Used";
            string awards;
            if (AlterRulesMenu.Instance.numAwards == 1)
            {
                awards = " award";
            }
            else
            {
                awards = " awards";
            }
            AlterRulesMenu.Instance.rulesCC2.text = AlterRulesMenu.Instance.numAwards + awards;
        }
        else
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On";
            AlterRulesMenu.Instance.rulesCC1.text = "Are NOT Being Used";
            AlterRulesMenu.Instance.rulesCC2.text = "";
        }
    }

    private void ScrewText()
    {
        if (AlterRulesMenu.Instance.screwMode)
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle Off";
            AlterRulesMenu.Instance.rulesCC1.text = "In Heaven";
        }
        else
        {
            AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On";
            AlterRulesMenu.Instance.rulesCC1.text = "In Hell";
        }
        AlterRulesMenu.Instance.rulesCC2.text = "";
    }

    IEnumerator AnimateSlider()
    {
        float rad = 0f;
        while (isChangingSize)
        {
            yield return null;
        }

        while (isSelected)
        {
            rad += Time.deltaTime * 3f;

            if (rad > (2 * Mathf.PI))
            {
                rad -= (2 * Mathf.PI);
            }

            float addScale = Mathf.Sin(rad) * 0.1f + 1.1f;

            transform.localScale = new Vector3(baseScale * addScale, baseScale * addScale, baseScale * addScale);
            yield return null;
        }
    }

    private void UpdateSelectedColors()
    {
        if (selectedColors.Count == 0)
        {
            selectedColors.Add(circle.color);
            selectedColors.Add(leftArrow.GetComponent<Image>().color);
            selectedColors.Add(rightArrow.GetComponent<Image>().color);
            selectedColors.Add(image.color);
            selectedColors.Add(text.color);
            if (toggle)
            {
                selectedColors.Add(toggle.graphic.color);
            }
        }
        else
        {
            selectedColors[0] = (circle.color);
            selectedColors[1] = (leftArrow.GetComponent<Image>().color);
            selectedColors[2] = (rightArrow.GetComponent<Image>().color);
            selectedColors[3] = (image.color);
            selectedColors[4] = (text.color);
            if (toggle)
            {
                selectedColors[5] = toggle.graphic.color;
            }
        }
    }

    private void UpdateColors(float multiplier)
    {
        if (circle && selectedColors[0] != null)
            circle.color = new Color(selectedColors[0].r * multiplier, selectedColors[0].g * multiplier, selectedColors[0].b * multiplier);
        if (leftArrow && selectedColors[1] != null)
            leftArrow.GetComponent<Image>().color = new Color(selectedColors[1].r * multiplier, selectedColors[1].g * multiplier, selectedColors[1].b * multiplier);
        if (rightArrow && selectedColors[2] != null)
            rightArrow.GetComponent<Image>().color = new Color(selectedColors[2].r * multiplier, selectedColors[2].g * multiplier, selectedColors[2].b * multiplier);
        if (image && selectedColors[3] != null)
            image.color = new Color(selectedColors[3].r * multiplier, selectedColors[3].g * multiplier, selectedColors[3].b * multiplier);
        if (text && selectedColors[4] != null)
            text.color = new Color(selectedColors[4].r * multiplier, selectedColors[4].g * multiplier, selectedColors[4].b * multiplier);
        if (toggle)
        {
            toggle.graphic.color = new Color(selectedColors[5].r * multiplier, selectedColors[5].g * multiplier, selectedColors[5].b * multiplier);
        }
    }
}
