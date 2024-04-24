using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AwardScoreMenu : MonoBehaviour
{
    private static AwardScoreMenu instance;
    public static AwardScoreMenu Instance {  get { return instance; } }

    [SerializeField] Slider scoreSlider;
    [SerializeField] List<AwardScores> awardScores;
    [SerializeField] GameObject allScores;
    [SerializeField] GameObject blackout;

    [SerializeField] Button playAgain;
    [SerializeField] TMP_Text awardName, calories;
    [SerializeField] TMP_Text description;

    public int selectedAward;

    public bool scoresMenu = false;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreSlider.maxValue = awardScores.Count - 1;
        scoreSlider.value = 0;
        //selectedAward = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Start();
    }

    public void SliderValueChange()
    {
        if (allScores.activeSelf)
        {
            for (int i = 0; i < awardScores.Count; i++)
            {
                if (scoreSlider.value == i)
                {
                    awardScores[i].highlight.enabled = true;
                }
                else
                {
                    awardScores[i].highlight.enabled = false;
                }
            }
        }

        allScores.transform.localPosition = new Vector2(743 - (237 * scoreSlider.value * allScores.transform.localScale.x), 9);
        UpdateText();
    }

    public void YPressed(InputAction.CallbackContext ctx)
    {
        if (AwardShowManager.Instance.awardsDone)
        {
            if (ctx.started)
            {
                scoresMenu = !scoresMenu;

                if (scoresMenu)
                {
                    blackout.SetActive(true);
                    scoreSlider.interactable = true;
                    scoreSlider.Select();
                    scoreSlider.value = 0;

                    if (allScores.activeSelf)
                    {
                        for (int i = 0; i < awardScores.Count; i++)
                        {
                            if (scoreSlider.value == i)
                            {
                                awardScores[i].highlight.enabled = true;
                            }
                            else
                            {
                                awardScores[i].highlight.enabled = false;
                            }
                        }

                        UpdateDescription();
                    }
                }
                else
                {
                    blackout.SetActive(false);
                    scoreSlider.interactable = false;
                    playAgain.Select();
                }
            }
        }
    }

    public void BPressed(InputAction.CallbackContext ctx)
    {
        if (AwardShowManager.Instance.awardsDone)
        {
            if (ctx.started)
            {
                if (scoresMenu)
                {
                    scoresMenu = false;

                    blackout.SetActive(false);
                    scoreSlider.interactable = false;
                    playAgain.Select();
                }
            }
        }
    }

    private void UpdateText()
    {
        if (awardScores[(int)scoreSlider.value].award != null)
        {
            awardName.text = "Award: " + awardScores[(int)scoreSlider.value].award.name;
            calories.text = "Calories: " + awardScores[(int)scoreSlider.value].award.calorieCountBonus + " x" + AwardShowManager.Instance.scoreMult;

            if (scoreSlider.value == 13)
            {
                awardName.text = "Award: Schmoovin / Schmoovless";
                calories.text = "Calories: " + awardScores[(int)scoreSlider.value].award.calorieCountBonus + " x" + AwardShowManager.Instance.scoreMult;
            }

            UpdateDescription();
        }
    }

    private void UpdateDescription()
    {
        switch (scoreSlider.value)
        {
            case 0:
                description.text = "Who stunned the most enemy Gulls?";
                break;
            case 1:
                description.text = "Who collected the most French Fries?";
                break;
            case 2:
                description.text = "Who used the most Power-Ups?";
                break;
            case 3:
                description.text = "Who was stunned the most?";
                break;
            case 4:
                description.text = "Who held the Golden Crust the longest?";
                break;
            case 5:
                description.text = "Who collected the most Chili Peppers?";
                break;
            case 6:
                description.text = "Who dealt the most fire damage?";
                break;
            case 7:
                description.text = "Who collected the most Popcorn?";
                break;
            case 8:
                description.text = "Who used the most Fireworks?";
                break;
            case 9:
                description.text = "Who collected the most Donuts?";
                break;
            case 10:
                description.text = "Who hit with Seagull Spin the most?";
                break;
            case 11:
                description.text = "Who hit with Seagull Smash the most?";
                break;
            case 12:
                description.text = "Who used Seagull Shot the most?";
                break;
            case 13:
                description.text = "Who moved the most / least?";
                break;
            case 14:
                description.text = "Who dealt the most damage overall?";
                break;
            case 15:
                description.text = "Who ended with the least Calories?";
                break;
        }
    }
}
