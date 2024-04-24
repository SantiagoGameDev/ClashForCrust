using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.InputSystem.LowLevel;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu instance;
    public static PauseMenu Instance { get { return instance; } }
    [SerializeField] private List<GameObject> pauseMenus;
    [SerializeField] private List<GameObject> resumeButtons;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] TMP_Text countText;

    private bool isPaused;
    private bool canPause;
    private bool countingDown;

    private bool pausingInProgress;

    public Slider pauseBufferBar;

    private int currentPlayerPaused = -1;

    private int currentPlayerHoldingPause = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        isPaused = false;
        canPause = true;
        countingDown = false;
        pausingInProgress = false;

        playerInputActions = new PlayerInputActions();
        playerInputActions.NoPowerUp.Enable();
    }

    public void PauseButtonPerformed(int playerNum)
    {
        if (!isPaused)
        {
            currentPlayerPaused = playerNum;
        }
        //Debug.Log("Performed");

        if (playerNum == currentPlayerPaused)
        {
            if (!RoundManager.Instance.GameStart)
            {
                if (!isPaused && canPause && !countingDown)
                {
                    StartCoroutine(EnablePause());
                    Pause();
                }
                else if (isPaused && canPause && !countingDown)
                {
                    StartCoroutine(EnablePause());
                    Resume();
                }
            }
        }
    }

    private void Pause()
    {
        isPaused = true;
        pauseMenus[currentPlayerPaused].SetActive(true);

        //Debug.Log("current player " + currentPlayerPaused);

        //EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < 4; i ++)
        {
            if (i == currentPlayerPaused)
            {
                RoundManager.Instance.players[i].GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
                RoundManager.Instance.players[i].mes.playerRoot = pauseMenus[i];
                RoundManager.Instance.players[i].mes.sendNavigationEvents = true;
                RoundManager.Instance.players[i].mes.firstSelectedGameObject = resumeButtons[i];
                RoundManager.Instance.players[i].mes.SetSelectedGameObject(RoundManager.Instance.players[i].mes.firstSelectedGameObject);
            }
            else
            {
                if (RoundManager.Instance.players[i])
                {
                    RoundManager.Instance.players[i].mes.playerRoot = pauseMenus[i];
                    RoundManager.Instance.players[i].mes.sendNavigationEvents = false;
                    RoundManager.Instance.players[i].mes.SetSelectedGameObject(null);
                }
            }
        }

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.Pause, true);

        Time.timeScale = 0f;
    }

    public void Resume()
    {

        RoundManager.Instance.players[currentPlayerPaused].mes.SetSelectedGameObject(null);
        RoundManager.Instance.players[currentPlayerPaused].mes.sendNavigationEvents = false;
        RoundManager.Instance.players[currentPlayerPaused].GetComponent<PlayerInput>().SwitchCurrentActionMap("NoPowerUp");

        pauseMenus[currentPlayerPaused].SetActive(false);

        StartCoroutine(CountDown());
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneControl.Instance.LoadSceneFromName("Menu");
    }

    public void StartPauseBar(InputAction.CallbackContext ctx, int playerNum)
    {
        pauseBufferBar.gameObject.SetActive(true);
        StartCoroutine(PauseBarProgress(ctx, playerNum));
    }

    public bool PauseBarStatus()
    {
        if (pauseBufferBar.gameObject.activeSelf)
            return true;
        else
            return false;
    }

    public IEnumerator PauseBarProgress(InputAction.CallbackContext ctx, int playerNum)
    {
        if (!pausingInProgress)
        {
            currentPlayerHoldingPause = playerNum;
            pausingInProgress = true;
        }

        while(pauseBufferBar.value < 2.0f && pausingInProgress && playerNum == currentPlayerHoldingPause)
        {
            pauseBufferBar.value += Time.deltaTime /2f;

            if (ctx.canceled)
            {
                pausingInProgress = false;
                currentPlayerHoldingPause = -1;
            }

            yield return new WaitForEndOfFrame();

        }
        
    }

    public void ResetPauseBuffer()
    {
        pauseBufferBar.value = pauseBufferBar.minValue;
        pausingInProgress = false;
        pauseBufferBar.gameObject.SetActive(false);

    }

    IEnumerator EnablePause()
    {
        canPause = false;

        yield return new WaitForSecondsRealtime(0.3f);

        canPause = true;
    }

    IEnumerator CountDown()
    {
        countingDown = true;
        int count = 3;

        countdownCanvas.SetActive(true);

        AudioManager.Instance.StartCountdown(1f, true);

        while (count > 0)
        {
            countText.text = count.ToString();
            StartCoroutine(TextAnim());
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countText.text = "Unfreeze!";
        StartCoroutine(TextAnim());

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.UnPause, true);


        isPaused = false;
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(1f);
        countdownCanvas.SetActive(false);
        countingDown = false;
    }

    IEnumerator TextAnim()
    {
        //current scale of the image
        float scale = 0;

        countText.transform.localScale = new Vector3(scale, scale, scale);

        //while the scale is greater than the end scale
        while (scale < 13)
        {
            //subtract from the scale
            scale += Time.unscaledDeltaTime * 52;

            //update to new scale
            countText.transform.localScale = new Vector3(scale, scale, 1f);
            //make the image darker proportional to its scale

            yield return new WaitForEndOfFrame();
        }
    }
}
