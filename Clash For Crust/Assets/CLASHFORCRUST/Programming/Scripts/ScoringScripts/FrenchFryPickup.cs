using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenchFryPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider seagull)
    {
        //Debug.Log("Fry picked up");
        PlayerController player = seagull.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            WorldData.Instance.UpdateStats(player.playerNum, WorldData.statsType.calorieCount, 1);
            WorldData.Instance.UpdateStats(player.playerNum, WorldData.statsType.friesEaten, 1);
            player.IncrementShotMeter();
            PowerupSpawner.Instance.fries.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
