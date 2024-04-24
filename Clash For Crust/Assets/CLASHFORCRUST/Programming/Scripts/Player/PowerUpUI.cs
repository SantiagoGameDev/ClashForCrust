using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    private Canvas canvas;

    [SerializeField] private Image powerupImage;

    [SerializeField] private Sprite donut, chiliPepper, popcorn, firework, arrow, orange, bagOBullets, rum, dynamite;

    [SerializeField] private GameObject featherUI;
    [SerializeField] private TMP_Text featherText;
    private PlayerController pc;

    private bool pirateMode;

    private int storedStamina = 5;

    private bool healthLost = false;

    PlayerController.ActivePowerup state;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

        featherUI.SetActive(false);
        canvas.worldCamera = WorldData.Instance.CurrentCamera;

        pc = GetComponentInParent<PlayerController>();

        powerupImage.sprite = arrow;

        switch (pc.playerNum)
        {
            case 0:
                powerupImage.color = Color.blue;
                break;
            case 1:
                powerupImage.color = Color.red;
                break;
            case 2:
                powerupImage.color = Color.green;
                break;
            case 3:
                powerupImage.color = Color.yellow;
                break;

        }

        if (AudioManager.Instance.pirateMode) //Check if we are on PiratePeak
            pirateMode = true;
        else
            pirateMode = false;

        powerupImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        StartCoroutine(StartingArrow());
    }

    // Update is called once per frame
    void Update()
    {
        if (pc)
        {
            state = pc.activePowerup;

            if (!RoundManager.Instance.hideHud)
            {
                if (!RoundManager.Instance.GameStart)
                {
                    switch (state)
                    {                            
                        case PlayerController.ActivePowerup.NONE:
                        case PlayerController.ActivePowerup.CRUST:
                            if (powerupImage.enabled)
                            {
                                StartCoroutine(UsedPowerup());
                            }
                            else
                            {
                                powerupImage.enabled = false;
                            }
                            break;
                        case PlayerController.ActivePowerup.DONUT:
                            if (pirateMode)
                                powerupImage.sprite = orange;
                            else
                                powerupImage.sprite = donut;

                            if (!powerupImage.enabled)
                            {
                                StartCoroutine(GotPowerup());
                            }
                            break;
                        case PlayerController.ActivePowerup.CHILIPEPPER:
                            if (pirateMode)
                                powerupImage.sprite = rum;
                            else
                                powerupImage.sprite = chiliPepper;

                            if (!powerupImage.enabled)
                            {
                                StartCoroutine(GotPowerup());
                            }
                            break;
                        case PlayerController.ActivePowerup.POPCORN:
                            if (pirateMode)
                                powerupImage.sprite = bagOBullets;
                            else
                                powerupImage.sprite = popcorn;

                            if (!powerupImage.enabled)
                            {
                                StartCoroutine(GotPowerup());
                            }
                            break;
                        case PlayerController.ActivePowerup.FIREWORK:
                            if (pirateMode)
                                powerupImage.sprite = dynamite;
                            else
                                powerupImage.sprite = firework;

                            if (!powerupImage.enabled)
                            {
                                StartCoroutine(GotPowerup());
                            }
                            break;
                    }

                    if (!healthLost & powerupImage.enabled)
                        powerupImage.color = Color.white;
                }
            }

            if (canvas.worldCamera)
            {
                transform.LookAt(transform.position + canvas.worldCamera.transform.rotation * Vector3.forward, canvas.worldCamera.transform.rotation * Vector3.up);
            }
        }
    }

    IEnumerator GotPowerup()
    {
        if (!RoundManager.Instance.hideHud)
        {
            powerupImage.enabled = true;

            float scale = 0f;
            while (scale < 1.1f)
            {
                scale += Time.deltaTime * 4f;

                if (scale > 1.1f)
                {
                    scale = 1.1f;
                }

                powerupImage.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            while (scale > 1f)
            {
                scale -= Time.deltaTime * 1f;

                if (scale < 1f)
                {
                    scale = 1f;
                }

                powerupImage.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            StartCoroutine(PowerupPop());
        }
    }

    IEnumerator PowerupPop()
    {
        while (state != PlayerController.ActivePowerup.NONE && state != PlayerController.ActivePowerup.CRUST)
        {
            float scale = 1.2f;
            while (scale > 1f)
            {
                scale -= Time.deltaTime * 0.6f;

                if (scale < 1f)
                {
                    scale = 1f;
                }

                powerupImage.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator UsedPowerup()
    {
        if (!RoundManager.Instance.hideHud)
        {
            float scale = 1f;
            while (scale > 0f)
            {
                scale -= Time.deltaTime * 4f;

                if (scale < 0f)
                {
                    scale = 0f;
                }

                powerupImage.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            powerupImage.enabled = false;
        }
    }

    public void UpdateStaminaValue()
    {
        if (!RoundManager.Instance.hideHud)
        {
            StartCoroutine(StaminaUI());
        }
    }

    IEnumerator StaminaUI()
    {
        healthLost = true;
        featherUI.SetActive(true);
        storedStamina = GetComponentInParent<PlayerController>().Stamina;
        featherText.text = storedStamina.ToString();
        float outtaFull = storedStamina / 5f;
        featherText.color = new Color(outtaFull, outtaFull, outtaFull);

        float scale = 0;

        while (scale < 1.1f)
        {
            scale += Time.deltaTime * 8f;

            if (scale > 1.1f)
            {
                scale = 1.1f;
            }

            if (powerupImage.enabled)
            {
                if (scale > 1f)
                {
                    powerupImage.color = new Color(powerupImage.color.r, powerupImage.color.g, powerupImage.color.b, 0.35f);
                }
                else
                {
                    powerupImage.color = new Color(powerupImage.color.r, powerupImage.color.g, powerupImage.color.b, 1f - scale * 0.65f);
                }
            }

            featherUI.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        while (scale > 1f)
        {
            scale -= Time.deltaTime * 8f;

            if (scale < 1f)
            {
                scale = 1f;
            }

            featherUI.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (scale > 0f)
        {
            scale -= Time.deltaTime * 8f;

            if (scale < 0f)
            {
                scale = 0f;
            }

            if (powerupImage.enabled)
            {
                if (scale < 0f)
                {
                    powerupImage.color = new Color(powerupImage.color.r, powerupImage.color.g, powerupImage.color.b, 1f);
                }
                else
                {
                    powerupImage.color = new Color(powerupImage.color.r, powerupImage.color.g, powerupImage.color.b, 1f - scale * 0.5f);
                }
            }

            featherUI.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        featherUI.gameObject.SetActive(false);
        healthLost = false;

        yield return null;
    }

    private IEnumerator StartingArrow()
    {
        while (RoundManager.Instance.GameStart)
        {
            powerupImage.enabled = !powerupImage.enabled;
            yield return new WaitForSeconds(0.5f);
        }

        powerupImage.color = Color.white;
        powerupImage.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
