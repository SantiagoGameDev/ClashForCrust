using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiliPepperPowerUp : MonoBehaviour
{
    private static ChiliPepperPowerUp instance;

    public static ChiliPepperPowerUp Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<ChiliPepperPowerUp>();
            }
            return instance;
        }
    }

    public void FireBreath()
    {
        gameObject.SetActive(true);
    }
}