using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornPowerUp : MonoBehaviour
{
    private static PopcornPowerUp instance;
    public int speed = 10;
    public int sender;
    public static PopcornPowerUp Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<PopcornPowerUp>();
            }
            return instance;
        }
    }
     private void OnTriggerEnter(Collider other)
    {
       // if (other.gameObject.CompareTag("Obstical")) //Destroy popcorn if it hits an obstical
       //     Destroy(gameObject);

        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (other.gameObject.GetComponent<PlayerController>().playerNum != sender)
            {
                Destroy(gameObject);
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }

        // Update is called once per frame
        void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
