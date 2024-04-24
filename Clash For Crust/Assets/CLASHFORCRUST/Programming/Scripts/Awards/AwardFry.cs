using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardFry : MonoBehaviour
{
    private float multiplier;
    private int playerNum;

    private float destroyY = 11f;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);
    }

    public void InitializeValues(int inPN, float inM)
    {
        playerNum = inPN;
        multiplier = inM;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < destroyY)
        {
            AwardShowManager.Instance.IncrementScore(playerNum, (int)multiplier);
            AwardShowManager.Instance.PlayEatSound();
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(0, -6500f * Time.fixedDeltaTime, 0), ForceMode.Impulse);
    }
}
