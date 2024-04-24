using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShotMeter : MonoBehaviour
{
    [SerializeField] private int playerNum;

    public Slider slider;
    public Image ball;
    public Image sliderOutline;
    public Image sliderFill;
    public GameObject sliderBackground;
    private bool isResetting = false;

    private float radius = 178f;

    float addAngle;

    private void Start()
    {

        addAngle = 0f;
        switch (playerNum)
        {
            case 0:
                addAngle = 0f;
                break;
            case 1:
                addAngle = -90f;
                break;
            case 2:
                addAngle = 90f;
                break;
            case 3:
                addAngle = 180f;
                break;
        }
        this.transform.Rotate(Vector3.forward, addAngle, Space.Self);
        slider.enabled = false;
        sliderOutline.enabled = false;
        sliderBackground.SetActive(false);
    }

    public void Update()
    {
        if (!RoundManager.Instance.GameStart)
        {
            if (playerNum < RoundManager.Instance.playersRequiredToStart && !RoundManager.Instance.hideHud)
            {
                slider.enabled = true;
                sliderOutline.enabled = true;
                sliderBackground.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        if (RoundManager.Instance.hideHud)
        {
            slider.enabled = false;
            ball.enabled = false;
            sliderOutline.enabled = false;
            sliderBackground.SetActive(false);
        }

        if (slider.value > 0.13 && !ball.enabled && !RoundManager.Instance.hideHud)
        {
            ball.enabled = true;
            UpdateBallPosition(slider.value);
        }
        else if (slider.value <= 0.13 && ball.enabled)
        {
            ball.enabled = false;
        }

        if (slider.value != 5)
        {
            ball.color = Color.white;
            sliderFill.color = Color.white;
        }

        UpdateBallPosition(slider.value);
    }

    public void IncreaseMeter(float valueToGoTo)
    {
        StartCoroutine(IncreaseAnimation(valueToGoTo));
    }

    public void DecreaseMeter()
    {
        StartCoroutine(DecreaseAnimation());
    }

    IEnumerator IncreaseAnimation(float endPos)
    {
        if (!RoundManager.Instance.hideHud)
        {
            float sliderPos = slider.value;

            while (isResetting)
            {
                yield return new WaitForEndOfFrame();
            }

            while (sliderPos < endPos)
            {
                sliderPos += Time.deltaTime * 3f;

                if (sliderPos > endPos)
                {
                    sliderPos = endPos;
                }

                slider.value = sliderPos;

                UpdateBallPosition(sliderPos);

                yield return new WaitForEndOfFrame();
            }

            if (slider.value == 5)
            {
                StartCoroutine(MeterFull());
            }
        }
    }

    IEnumerator DecreaseAnimation()
    {
        isResetting = true;

        float sliderPos = slider.value;

        while (sliderPos > 0) 
        {
            sliderPos -= Time.deltaTime * 15f;

            if (sliderPos < 0)
            {
                sliderPos = 0;
            }

            slider.value = sliderPos;

            UpdateBallPosition(sliderPos);

            yield return new WaitForEndOfFrame();
        }

        isResetting = false;
    }

    private void UpdateBallPosition(float inValue)
    {
        float maxPos = slider.maxValue;

        float angle = - ((inValue / maxPos) * (2*Mathf.PI)) - 0.05f;

        angle += transform.rotation.z * Mathf.Deg2Rad;

        angle += addAngle * Mathf.Deg2Rad;

        float rewRad = radius * ((float)Screen.width / 1920f);

        float xPos = Mathf.Cos(angle) * rewRad;
        float yPos = Mathf.Sin(angle) * rewRad;

        ball.transform.position = new Vector3(xPos, yPos) + transform.position;
    }

    IEnumerator MeterFull()
    {
        float percentWhite = 0f;

        while (slider.value == 5)
        {
            percentWhite += Time.deltaTime * 6f;

            if (percentWhite > Mathf.PI * 2f)
            {
                percentWhite = 0f;
            }

            float cv = 0.8f + Mathf.Cos(percentWhite) / 5f;

            ball.color = new Color(cv, cv, cv);
            sliderFill.color = new Color(cv, cv, cv);

            yield return null;
        }
    }

}
