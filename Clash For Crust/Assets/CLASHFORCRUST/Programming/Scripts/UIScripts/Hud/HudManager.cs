//Author: Branson Vernon
//Date: 01/30/24
//Purpose: Displays the HUD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class HudManager : MonoBehaviour
{
    private static HudManager instance;

    public static HudManager Instance
    {
        get { return instance; }
    }

    //Text that displays the time
    [SerializeField] private TMP_Text timerText;

    //The player circles
    [SerializeField] public List<GameObject> playerCircles;

    [SerializeField] private List<ShotMeter> PlayerShotMeters;


    //The sprite the stamina icons will use
    public Sprite popcornSprite;

    //scale value for a stamina icon at normal size
    private float fullSizeSI = 1.25f;
    public float FullSizeSI { get { return fullSizeSI; } }

    [SerializeField]
    private TMP_Text countdownText;

    [SerializeField]
    private GameObject featherObject;

    //instance = this
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < playerCircles.Count; i++)
        {
            playerCircles[i].gameObject.SetActive(false);
        }

        timerText.enabled = false;
        //Get the player hud objects and arrange them
    }

    //debug button function to start the timer
    public void StartTimer()
    {
        StartCoroutine(UpdateTimerText());
    }

    //Updates the text for the timer
    private IEnumerator UpdateTimerText()
    {
        //while the game is still happening
        while (!WorldData.Instance.gameOver)
        {
            //get timerValue from worldData
            float timerValue = Mathf.Round(WorldData.Instance.GetGameTime() + 0.03f);

            //Calculate minute and seconds values
            int minutes = (int)(timerValue / 60);
            int seconds = (int)(timerValue % 60);

            //Check if there should be a bonus zero before the minute value
            string bonusZero = "";
            if (seconds < 10)
            {
                bonusZero = "0";
            }

            if (!RoundManager.Instance.hideHud)
            {
                //Update the timerText
                timerText.text = minutes + ":" + bonusZero + seconds;
            }
            else
            {
                timerText.enabled = false;
            }

            //Wait a second before updating it again
            yield return new WaitForSeconds(1f);
        }

        //Set timerText to 0:00 when game is over
        timerText.text = "0:00";
    }

    public void StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        AudioManager.Instance.StartCountdown(1f);

        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        //StartCoroutine(IntroLine());

        if (!RoundManager.Instance.joinOnButton)
        {
            int count = 3;

            while (count > 0)
            {
                countdownText.text = count.ToString();
                StartCoroutine(CountdownAnim());
                yield return new WaitForSeconds(1f);

                count--;
            }
        }

        countdownText.text = "CLASH!";
        StartCoroutine(CountdownAnim());

        for (int i = 0; i < playerCircles.Count; i++)
        {
            if (i >= RoundManager.Instance.playersRequiredToStart)
            {
                playerCircles[i].gameObject.SetActive(false);
            }
            else
            {
                playerCircles[i].gameObject.SetActive(true);
                for (int j = 0; j < RoundManager.Instance.MaxHealth; j++)
                {
                    GameObject feather = Instantiate(featherObject, playerCircles[i].transform);
                    feather.name = "P" + i + "F" + (j + 1);
                }
            }

            if (RoundManager.Instance.hideHud)
            {
                playerCircles[i].gameObject.SetActive(false);
            }
        }



        if (RoundManager.Instance.introLineDone)
        {
            RoundManager.Instance.StartMatch();
            timerText.enabled = true;

            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeCountdown());
        }

    }

    IEnumerator CountdownAnim()
    {
        //current scale of the image
        float scale = 0;

        countdownText.transform.localScale = new Vector3(scale, scale, scale);

        //while the scale is greater than the end scale
        while (scale < 7)
        {
            //subtract from the scale
            scale += Time.unscaledDeltaTime * 28;

            //update to new scale
            countdownText.transform.localScale = new Vector3(scale, scale, 1f);
            //make the image darker proportional to its scale

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeCountdown()
    {
        //current scale of the image
        float alpha = 1f;

        Color baseColor = countdownText.color;

        //while the scale is greater than the end scale
        while (alpha > 0)
        {
            //subtract from the scale
            alpha -= Time.deltaTime * 1.2f;

            if (alpha < 0)
            {
                alpha = 0;
            }
            //update to new scale
            countdownText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            //make the image darker proportional to its scale

            yield return new WaitForEndOfFrame();
        }

        countdownText.gameObject.SetActive(false);
    }

    public void IncreaseShotMeter(int playerIndex, int shotValue)
    {
        PlayerShotMeters[playerIndex].IncreaseMeter(shotValue);
    }

    public void ResetShotMeter(int playerIndex)
    {
        PlayerShotMeters[playerIndex].DecreaseMeter();
    }

    public IEnumerator IntroLine()
    {
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.BattleIntro, true);

        yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());

        if (RoundManager.Instance.CrustOn)
            StartCoroutine(RoundManager.Instance.CameraZoomOut());

        //yield return new WaitForSeconds(3f);

        //Debug.Log("Intro line finished");
        RoundManager.Instance.introLineDone = true;
        StartCountdown();
    }
}