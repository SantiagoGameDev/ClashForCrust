using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePowerupSpawner : MonoBehaviour
{
    private static WavePowerupSpawner instance;

    public static WavePowerupSpawner Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<WavePowerupSpawner>();
            }
            return instance;
        }
    }

    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;

    private bool point1Taken = false;
    private bool point2Taken = false;
    private bool point3Taken = false;
    private bool point4Taken = false;

    public GameObject firework;
    public GameObject pepper;
    public GameObject donut;
    public GameObject popcorn;
    public GameObject screw;

    public void SpawnPowerups()
    {
        if (RoundManager.Instance.PowerOn)
        {
            int numOfPowerups = Random.Range(0, 4);

            switch (numOfPowerups)
            {
                case 0:
                    SpawnPowerup();
                    break;

                case 1:
                    SpawnPowerup();
                    SpawnPowerup();
                    break;

                case 2:
                    SpawnPowerup();
                    SpawnPowerup();
                    SpawnPowerup();
                    break;

                case 3:
                    SpawnPowerup();
                    SpawnPowerup();
                    SpawnPowerup();
                    SpawnPowerup();
                    break;
            }

            point1Taken = false;
            point2Taken = false;
            point3Taken = false;
            point4Taken = false;
        }
    }

    private void SpawnPowerup()
    {
        GameObject randomPoint = GetRandomPoint();
        GameObject randomPowerup = GetRandomPowerup();

        if (!RoundManager.Instance.screwMode)
            Instantiate(randomPowerup, randomPoint.transform.position, Quaternion.identity, transform);
        else
            Instantiate(screw, randomPoint.transform.position, Quaternion.identity, transform);
    }

    private GameObject GetRandomPoint()
    {
        int randNum = Random.Range(0, 4);

        while ((randNum == 0 && point1Taken) || (randNum == 1 && point2Taken) || (randNum == 2 && point3Taken) || (randNum == 3 && point4Taken))
        {
            randNum = Random.Range(0, 4);
        }

        switch (randNum)
        {
            case 0:
                point1Taken = true;
                return point1;

            case 1:
                point2Taken = true;
                return point2;

            case 2:
                point3Taken = true;
                return point3;

            case 3:
                point4Taken = true;
                return point4;

            default:
                return point1;
        }
    }

    private GameObject GetRandomPowerup()
    {
        int randNum = Random.Range(0, 4);
        switch (randNum)
        {
            case 0:
                return firework;

            case 1:
                return pepper;

            case 2:
                return donut;

            case 3:
                return popcorn;

            default:
                return firework;
        }
    }
}