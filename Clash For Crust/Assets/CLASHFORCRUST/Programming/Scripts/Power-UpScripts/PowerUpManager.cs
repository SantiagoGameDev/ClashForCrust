using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject donutGO;

    //public GameObject chiliPepperGO;

    public GameObject popcornGo;

    private static PowerUpManager instance;

    public static PowerUpManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<PowerUpManager>();
            }
            return instance;
        }
    }

    bool pirateMode;

    private void Start()
    {
        // set powerups to not be active
        //chiliPepperGO.SetActive(false);
    }

    public void DonutActive(GameObject Sender)
    {
        GameObject SpawnedDonut = Instantiate(donutGO);
        SpawnedDonut.transform.position = Sender.GetComponent<PlayerController>().beakPos.transform.position;// + new Vector3(1, 0.75f, 0);
        SpawnedDonut.transform.rotation = Quaternion.Euler(new Vector3(360, Sender.transform.rotation.eulerAngles.y, 90));
        SpawnedDonut.GetComponentInParent<DonutPowerUp>().sender = Sender.GetComponent<PlayerController>().playerNum;

        WorldData.Instance.UpdateStats(Sender.GetComponent<PlayerController>().playerNum, WorldData.statsType.powerups, 1);
    }

    public void ChiliPepperActive(GameObject player)
    {
        ChilliBreath chili = player.GetComponentInChildren<ChilliBreath>();
        chili.ChilliBreathAttack();
        WorldData.Instance.UpdateStats(player.GetComponent<PlayerController>().playerNum, WorldData.statsType.powerups, 1);
    }

    public void FireworkActive(GameObject player)
    {
        Firework firework = player.GetComponent<Firework>();
        firework.FireworkAttack();
        WorldData.Instance.UpdateStats(player.GetComponent<PlayerController>().playerNum, WorldData.statsType.powerups, 1);
    }

    public void PopcornActive(GameObject Sender)
    {
        //Debug.LogWarning("did we get here?" + Sender.transform.rotation.eulerAngles.y);
        GameObject SpawnedKernal = Instantiate(popcornGo);
        SpawnedKernal.transform.rotation = Quaternion.Euler(new Vector3(0, Sender.transform.rotation.eulerAngles.y, 0));
        Transform beakLoc = Sender.GetComponent<PlayerController>().beakPos.transform;
        SpawnedKernal.transform.position = beakLoc.position;
        SpawnedKernal.GetComponentInParent<PopcornPowerUp>().sender = Sender.GetComponent<PlayerController>().playerNum;
    }
}