using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class Feathers : MonoBehaviour
{
    //which player these feathers are set to, and which player
    public int playerNum, iconNum;

    private bool isHealing, isLosing;
    private int currentHealth;

    private Vector3 basePosition;
    private float baseAngle;
    private float fullSize;

    private int fullHealth = 5;

    [SerializeField] private Image iconImage, circle;

    private bool gameStarted;
    private bool extraCircle;

    void Start()
    {

        isHealing = false;
        isLosing = false;
        gameStarted = false;

        fullHealth = RoundManager.Instance.MaxHealth;

        playerNum = int.Parse(name.Substring(name.IndexOf("P") + 1, 1));
        iconNum = int.Parse(name.Substring(name.IndexOf("F") + 1));

        fullSize = HudManager.Instance.FullSizeSI;
        iconImage.sprite = HudManager.Instance.popcornSprite;

        FeatherEvents.Instance.OnPlayerGainedHealth += Healing;
        FeatherEvents.Instance.OnPlayerLostHealth += LosingHealth;

        GetBasePosition();

        switch (playerNum)
        {
            case 0:
                circle.color = Color.blue;
                break;
            case 1:
                circle.color = Color.red;
                break;
            case 2:
                circle.color = Color.green;
                break;
            case 3:
                circle.color = Color.yellow;
                break;
        }

        iconImage.enabled = false;
        circle.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!RoundManager.Instance.GameStart && !gameStarted)
        {
            if (playerNum < RoundManager.Instance.playersRequiredToStart)
            {
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
                gameObject.SetActive(false);
            }

            gameStarted = true;
        }

        if (RoundManager.Instance.hideHud)
        {
            iconImage.enabled = false;
        }

        if (RoundManager.Instance.players[playerNum] != null)
        {
            currentHealth = RoundManager.Instance.players[playerNum].Stamina;

            if (!isLosing && !isHealing)
            {
                if (currentHealth >= iconNum)
                {
                    iconImage.enabled = true;

                    if (!extraCircle)
                    {
                        circle.enabled = false;
                    }
                    if (transform.position != basePosition)
                    {
                        transform.position = basePosition;
                    }
                }
                else if (currentHealth < iconNum)
                {
                    iconImage.enabled = false;
                }
            }

            if (!isLosing)
            {
                if (currentHealth >= iconNum)
                {
                    UpdateIdleRotation();
                }
            }
        }
    }

    private void UpdateIdleRotation()
    {
        float newAngle = FeatherEvents.Instance.Timer;

        float addAngle = Mathf.Sin(newAngle) * 15f;

        transform.rotation = Quaternion.Euler(0, 0, baseAngle + addAngle);
    }

    private void GetBasePosition()
    {
        //value to scale the stamina icons by
        transform.localScale = new Vector3(fullSize, fullSize, 1f);

        float addAngle = 0;
        switch (playerNum)
        {
            //player 1 adds 270 degrees
            case 0:
                addAngle = Mathf.PI * 3 / 2;
                break;
            //player 2 adds 180 degrees
            case 1:
                addAngle = Mathf.PI;
                break;
            //player 4 adds 90 degrees
            case 3:
                addAngle = Mathf.PI * 1 / 2;
                break;
        }

        //the angle of the circle that the icon is on
        //(90 degrees / 10 * 2i + 1) + the angle
        // so 9, 27, 45, 63, 81 + the angle
        //haha
        float angle = ((Mathf.PI / 2) / (2 * fullHealth) * (2 * iconNum - 1) + addAngle);
        //the radius of the circle for how far out the icons should be from the corner
        float radius = 228f * ((float)Screen.width / 1920f);


        //the circle's position so they aren't all placed around 1 circle
        Vector3 circlePos = HudManager.Instance.playerCircles[playerNum].transform.position;

        //getting the actual positions based on the angle, radius and circle position
        float x = Mathf.Cos(angle) * radius + circlePos.x;
        float y = Mathf.Sin(angle) * radius + circlePos.y;

        //set the icon's position
        transform.position = new Vector3(x, y, 0);
        basePosition = transform.position;

        //set the icon's rotation
        angle *= Mathf.Rad2Deg;
        baseAngle = angle;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void LosingHealth(int player)
    {
        if (gameObject.activeSelf)
        {
            if (playerNum == player)
            {
                if (!isLosing && iconImage.enabled)
                {
                    currentHealth = RoundManager.Instance.players[playerNum].Stamina;

                    //Debug.Log("Current Health: " + currentHealth + "  IconNum: " + iconNum);

                    if (currentHealth < iconNum)
                    {
                        //Debug.Log("Health is less than iconNum");
                        StartCoroutine(LostHealth());
                    }
                }
            }
        }
    }

    private void Healing(int player)
    {
        if (gameObject.activeSelf)
        {
            if (playerNum == player)
            {
                currentHealth = RoundManager.Instance.players[playerNum].Stamina;

                if (currentHealth == iconNum)
                {
                    if (!isHealing)
                    {
                        StartCoroutine(GrowBackIn());
                    }
                }
            }
        }
    }

    //Called when health is less than previous frame
    IEnumerator LostHealth()
    {
        float radAngle = baseAngle * Mathf.Deg2Rad;
        
        isLosing = true;

        float gScale = 1f;
        while (gScale > 0.8f)
        {
            gScale -= Time.deltaTime * 1f;

            iconImage.transform.localScale = new Vector3(gScale, gScale, 0f);

            yield return new WaitForEndOfFrame();
        }

        float multi = 0f;
        while (multi < 1)
        {
            if (gScale < 1f)
            {
                gScale += Time.deltaTime * 1.3f;
                iconImage.transform.localScale = new Vector3(gScale, gScale, 1f);
            }
            multi += Time.deltaTime * (2.4f / (multi + 0.6f));

            float distance = 85f * multi;

            float x = Mathf.Cos(radAngle) * distance + basePosition.x;
            float y = Mathf.Sin(radAngle) * distance + basePosition.y;

            transform.position = new Vector3(x, y, 0f);

            yield return null;
        }

        gScale = 1f;
        while (gScale < 1.1f)
        {
            gScale += Time.deltaTime;
            if (gScale > 1.1f)
            {
                gScale = 1.1f;
            }

            iconImage.transform.localScale = new Vector3(gScale, gScale, 0f);

            yield return new WaitForEndOfFrame();
        }

        circle.enabled = true;
        circle.transform.position = iconImage.transform.position;

        float scale = 0f;
        while (scale < 1f)
        {
            scale += Time.deltaTime * 2f;

            circle.transform.localScale = new Vector3(3f * scale, 3f * scale, 0f);

            float scaleMulti = 2f;

            if (scale < 1.1f / scaleMulti)
            {
                iconImage.transform.localScale = new Vector3(1.1f - scale * scaleMulti, 1.1f - scale * scaleMulti, 0f);
            }

            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f - scale);

            yield return new WaitForEndOfFrame();
        }

        circle.enabled = false;
        isLosing = false;
    }

    //Called when health is more than the previous frame
    IEnumerator GrowBackIn()
    {
        isHealing = true;

        transform.position = basePosition;
        iconImage.enabled = true;
        iconImage.transform.localScale = new Vector3(1f, 1f, 0f);

        float scale = 0f;
        while (scale < 1.1f)
        {
            scale += Time.deltaTime * 1.8f;

            if (scale > 1.1f)
            {
                scale = 1.1f;
            }

            transform.localScale = new Vector3(scale * fullSize, scale * fullSize, 0f);

            yield return new WaitForEndOfFrame();
        }

        while (scale > 1f)
        {
            scale -= Time.deltaTime * 1.1f;

            if (scale < 1f)
            {
                scale = 1f;
            }

            transform.localScale = new Vector3(scale * fullSize, scale * fullSize, 0f);

            yield return new WaitForEndOfFrame();
        }

        isHealing = false;

        if (iconNum == fullHealth)
        {
            extraCircle = true;

            circle.enabled = true;
            circle.transform.position = HudManager.Instance.playerCircles[playerNum].transform.position;

            //yield return new WaitForSeconds(0.5f);

            scale = 0f;
            while (scale < 1f)
            {
                scale += Time.deltaTime * 2f;

                circle.transform.localScale = new Vector3(20f * scale, 20f * scale, 0f);

                circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f - scale);

                yield return new WaitForEndOfFrame();
            }

            circle.enabled = false;
            extraCircle = false;
        }
    }
}
