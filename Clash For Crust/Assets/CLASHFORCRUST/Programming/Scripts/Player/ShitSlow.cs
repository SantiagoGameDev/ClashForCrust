using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShitSlow : MonoBehaviour
{
    private float originalSpeed;

    private Collider playerCollider; // used to have a saved version of the player for when object is destroyed

    public SpriteRenderer shotPlayerIndicator;
    public int playerNum;

    private void Start()
    {
        originalSpeed = 7153; // keep updated with the speed value in editor (or just make the code good, idiot)
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2") || other.gameObject.CompareTag("Player3") || other.gameObject.CompareTag("Player4"))
        {
            if (shotPlayerIndicator.color == Color.blue && other.gameObject.GetComponent<PlayerController>().tag != "Player1")
            {
                other.gameObject.GetComponent<PlayerController>().speed = 3000;
            }
            else if (shotPlayerIndicator.color == Color.red && other.gameObject.GetComponent<PlayerController>().tag != "Player2")
            {
                other.gameObject.GetComponent<PlayerController>().speed = 3000;
            }
            else if (shotPlayerIndicator.color == Color.green && other.gameObject.GetComponent<PlayerController>().tag != "Player3")
            {
                other.gameObject.GetComponent<PlayerController>().speed = 3000;
            }
            else if (shotPlayerIndicator.color == Color.yellow && other.gameObject.GetComponent<PlayerController>().tag != "Player4")
            {
                other.gameObject.GetComponent<PlayerController>().speed = 3000;
            }

            playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2") || other.gameObject.CompareTag("Player3") || other.gameObject.CompareTag("Player4"))
        {
            other.gameObject.GetComponent<PlayerController>().speed = originalSpeed;
        }
    }

    private void OnDisable()
    {
        if (playerCollider != null)
            playerCollider.gameObject.GetComponent<PlayerController>().speed = originalSpeed;
    }
}