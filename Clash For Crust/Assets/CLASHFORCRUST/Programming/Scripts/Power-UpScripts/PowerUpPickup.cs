using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    //the rigidbody component on the powerup
    private Rigidbody body;

    //the direction the powerup is being thro
    [SerializeField]
    private Vector3 direction = new Vector3();

    private float speed = 5f;
    private float lifeTime;
    private float timer;
    [SerializeField] float lifeTimeVal;

    public bool canBePickedUp = false;
    private bool floorTouched;
    [SerializeField] private bool canDespawn = true;

    [SerializeField] private GameObject model;

    [SerializeField] private bool rotateY;

    public bool isFry;
    public bool isCrust;

    [SerializeField] private ParticleSystem pickupFx;

    private float crustRespawnTime = 15f;
    private float crustRepsawnTimer = 0f;

    private void Start()
    {
        floorTouched = false;
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (model != null)
        {
            if (!rotateY)
            {
                model.transform.Rotate(Vector3.forward, 90f * Time.deltaTime, Space.Self);
            }
            else
            {
                model.transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            }
        }

        OutOfBounds();
        LifeTimeDespawn();
        PickupOverride();

        if (isCrust)
        {
            crustRepsawnTimer += Time.deltaTime;
            if (crustRepsawnTimer >= crustRespawnTime)
            {
                crustRespawnTime = 0f;
                this.transform.position = GameObject.FindGameObjectWithTag("CrustSpawn").transform.position;
            }
                
        }

        body.AddForce(new Vector3(0, -1f * Time.deltaTime, 0f), ForceMode.Impulse);


    }

    private void OnDestroy()
    {
        if (isFry)
        {
            PowerupSpawner.Instance.fries.Remove(gameObject);
        }
        else
        {
            PowerupSpawner.Instance.powerups.Remove(this.gameObject);
        }
    }

    public void SetDirection(Vector3 dir, float inSpeed)
    {
        direction = dir;
        speed = inSpeed;

        body = GetComponent<Rigidbody>();

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        body.AddForce(direction * (speed), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            floorTouched = true;
            canBePickedUp = true;
            direction = Vector3.zero;
            body.velocity = Vector3.zero;
            if(pickupFx != null)
            {
                pickupFx.Play();
            }
            
        }

        if (collision.gameObject.layer == 8)
        {
            body.AddForce(direction * speed / 2, ForceMode.Impulse);
        }
    }

    public void CollidedWithPlayer(PlayerController player)
    {
        if (isFry)
        {
            //Debug.Log("Fry Picked up");
            WorldData.Instance.UpdateStats(player.playerNum, WorldData.statsType.calorieCount, 1);
            WorldData.Instance.UpdateStats(player.playerNum, WorldData.statsType.friesEaten, 1);
            FeatherEvents.Instance.ThisPlayerGainedCalories(player.playerNum);
            player.IncrementShotMeter();
            Destroy(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OutOfBounds()
    {
        if(transform.position.y < -10 && canDespawn)
        {
            Destroy(gameObject);
        }
    }

    private void LifeTimeDespawn()
    {
        if (floorTouched && canDespawn && !isCrust)
        {
            lifeTime += Time.deltaTime;

            if (lifeTime >= lifeTimeVal)
                Destroy(gameObject);
        }
    }

    private void PickupOverride() //If a pickup has existed for 2 or more seconds make it pickupable
    {
        if (!canBePickedUp)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
                canBePickedUp = true;
        }
    }
}