using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSlidingObstacle : MonoBehaviour
{
    [SerializeField] BoatRockingManager boatRockingManager;

    private void Awake()
    {
        if (!boatRockingManager)
            boatRockingManager = FindObjectOfType<BoatRockingManager>();
    }

    public void ChangeObjectPos(BoatRockingManager.shipStateType shipState)
    {

    }

}
