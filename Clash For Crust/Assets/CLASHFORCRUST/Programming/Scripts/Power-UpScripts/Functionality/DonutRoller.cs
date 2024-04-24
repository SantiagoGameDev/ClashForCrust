using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRoller : MonoBehaviour
{

    public float degreesPerSecond = 20;
    public int speed = 2;
    private void Update()
    {
        transform.Rotate(new Vector3(0, -degreesPerSecond, 0) * speed *Time.deltaTime); // spin donut
    }
}
