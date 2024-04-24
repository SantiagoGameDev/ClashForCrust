//Author: Branson Vernon
//Date: 01/30/24
//Purpose: updates the player's calorie count when the player is in crust mode,
//and throws the crust away when the player gets hit

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using static PlayerController;

public class CrustMode : MonoBehaviour
{
    //reference of the player's controller
    [SerializeField]
    PlayerController pc;

    //timer to count score being increased
    private float timer;

    //time till points increased
    [SerializeField]
    private float timeToPoints;

    //this players id
    private int playerNum;

    //get the player controller on awake
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        playerNum = pc.playerNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.activePowerup == PlayerController.ActivePowerup.CRUST)
        {
            WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalCrustTime, Time.deltaTime);
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.calorieCount, WorldData.Instance.GetCrustValue());
                FeatherEvents.Instance.ThisPlayerGainedCalories(playerNum);
                timer = 0f;
            }
        }
    }

    //when the object is enabled, start the point timer
    public void OnEnable()
    {
        timer = 0f;
        pc = GetComponent<PlayerController>();
        playerNum = pc.playerNum;
    }

    //called when the player loses the crust
    public void LoseCrust()
    {
        //no player is holding the crust
        RoundManager.Instance.SetHoldingCrust(-1);

        //get where the crust is being thrown
        Vector3 targetPos = GetDropPosition();

        //get angle to throw crust at
        Vector3 direction = (RoundManager.Instance.middle.position - transform.position) * 0.25f;
        direction.y = 6f;
        direction.Normalize();

        //get speed to throw crust at
        float speed = Random.Range(20f, 25f);

        GameObject goldenCrust;
        //spawn crust
        if (!RoundManager.Instance.screwMode)
            goldenCrust = Instantiate(RoundManager.Instance.goldenCrust, this.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
        else
            goldenCrust = Instantiate(RoundManager.Instance.goldenScrew, this.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);

        goldenCrust.transform.parent = RoundManager.Instance.transform;

        //get it to throw properly in its script
        PowerUpPickup scriptPU = goldenCrust.GetComponent<PowerUpPickup>();
        scriptPU.SetDirection(direction, speed);

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.DropCrust, false);

        if (!RoundManager.Instance.screwMode)
            pc.crustObject.SetActive(false);
        else
            pc.screwObject.SetActive(false);
        //pc.activePowerup = ActivePowerup.NONE;
        //disable this script
        this.enabled = false;
    }

    //Gets where the crust is thrown to
    private Vector3 GetDropPosition()
    {
        Vector3 pos = new Vector3();

        //get random angle between 1 and 360
        int angle = Random.Range(1, 361);

        float radius = 3f;

        //gets x and z positions on a circle thats got a radius of the variable radius
        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        //endPoint is the startPoint + those new x and z positions
        pos = this.transform.position + new Vector3(x, 0f, z);

        return pos;
    }
}
