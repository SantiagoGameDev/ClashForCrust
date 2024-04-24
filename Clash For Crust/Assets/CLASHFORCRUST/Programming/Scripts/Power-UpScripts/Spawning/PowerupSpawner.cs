//Author: Branson Vernon
//Date: 01/15/2024
//Purpose: Spawn powerups in from offscreen (or a specific spot maybe), the newest version of the PowerupSpawner script

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PowerupSpawner : MonoBehaviour
{
    //enumerated type to make powerup identifiers easier to view in code
    private enum PowerupType { None = -1, Popcorn = 0, Fireworks = 1, Donut = 2, Pepper = 3, Fry = 4}

    //current instance of the powerup spawner
    private static PowerupSpawner instance;
    public static PowerupSpawner Instance
    { 
        get { return instance; }
    }

    //spawnTime = time between powerup spawns
    //timer is self explanatory
    [SerializeField]
    private float spawnTime = 5f;
    private float frySpawnTime;
    public float timer;
    public float fryTimer;

    //maxPowerups is the total number of powerups that can exist at once
    [SerializeField]
    private int maxPowerups = 4;
    public List<GameObject> powerups = new List<GameObject>();

    [SerializeField]
    private int maxFries = 15;
    public List<GameObject> fries = new List<GameObject>();

    //Prefabs for the different powerup pickups
    [SerializeField]
    private List<GameObject> powerupPrefabs;
    [SerializeField] private GameObject fryPrefab;

    //Where to spawn the specific powerups from
    private PopcornMachine popcornSpawn;
    private Vector3 popcornSpawnPos;

    //the radius of the cirlce the powerups are spawning from
    [SerializeField]
    private float radius = 5;

    //testing button
    public Button button;

    public bool canSpawn;

    private int minAngle, maxAngle;

    private GameObject mapGround;
    private Vector3 mapCenter;

    [SerializeField] GameObject screwPickup;

    //at start
    private void Start()
    {
        mapGround = GameObject.FindGameObjectWithTag("Ground");
        mapCenter = mapGround.transform.position + new Vector3(0f, 3f, 0f);

        //Initialize Values
        timer = 0f;
        canSpawn = false;
    }
    private void Awake()
    {
        //instance is this
        if (instance == null)
        {
            instance = this;
        }
    }

    //Sets the spawn frequency of the PowerUps
    public void SetSpawnFrequency()
    {
        spawnTime = WorldData.Instance.GetPowerSpawnFrequency();
    }

    public void SetFryFrequency()
    {
        frySpawnTime = RoundManager.Instance.FrySpawnFrequency;
    }

    public void SetMaxPowerups()
    {
        maxPowerups = WorldData.Instance.MaxPowerups;
    }

    //Gets the current number of powerups in existence
    public int TotalPowerups()
    {
        return powerups.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            //timer incrementing
            timer += Time.deltaTime;
            fryTimer += Time.deltaTime;

            //when the timer hits the spawn time
            if (timer > spawnTime)
            {
                //if there are currently less powerups than the max number of powerups
                if (TotalPowerups() < maxPowerups)
                {
                    if (RoundManager.Instance.PowerOn)
                    {
                        //spawn a powerup
                        SpawnPowerup();
                    }
                }

                //reset the timer
                timer = 0f;
            }

            if (fryTimer > frySpawnTime)
            {
                if (fries.Count < maxFries)
                {
                    if (RoundManager.Instance.FryOn)
                    {
                        SpawnFry();
                    }
                }

                fryTimer = 0f;
            }
        }
    }

    private void SpawnFry()
    {
        bool difSpawn = false;
        //Get the point to launch the power up from, and where its aiming towards
        Vector3 startPoint = GetStartPoint(PowerupType.Fry, difSpawn);
        Vector3 endPoint = GetEndPoint(PowerupType.Fry, difSpawn);

        //Get aiming vector for when throwing the powerup
        Vector3 direction = endPoint - startPoint;
        direction.Normalize();
        if (difSpawn)
        {
            direction.y = 0.75f;
        }
        else
        {
            direction.y = 0.5f;
        }

        //Get speed to throw the powerup with
        float speed = GetSpeed(difSpawn);

        GameObject spawnedFry;
        //Insantiate the powerup gameobject, get its script, and call the set direction script to get it thrown
        if (!RoundManager.Instance.screwMode)
            spawnedFry = Instantiate(fryPrefab, startPoint, Quaternion.identity);
        else
            spawnedFry = Instantiate(screwPickup, startPoint, Quaternion.identity);
        PowerUpPickup scriptFry = spawnedFry.GetComponent<PowerUpPickup>();
        scriptFry.SetDirection(direction, speed);

        fries.Add(spawnedFry);
    }

    //The entire process of spawning in a powerup
    private void SpawnPowerup()
    {
        //Store the powerup ID so we know specifically which one was chosen
        PowerupType powID = PowerupType.None;

        //Whether a different spawn point is being used
        bool difSpawn = false;

        //Determine which powerup is being spawned in
        GameObject powerup = ChoosePowerup(ref powID);

        if (powerup != null)
        {
            //if power up is popcorn
            if (powID == PowerupType.Popcorn && popcornSpawn != null)
            {
                //get 0 or 1
                int rand = Random.Range(0, 2);

                //if 0 spawn normally, else spawn from a specific point
                if (rand == 0)
                {
                    difSpawn = false;
                }
                else
                {
                    difSpawn = true;
                }
            }

            //Get the point to launch the power up from, and where its aiming towards
            Vector3 startPoint = GetStartPoint(powID, difSpawn);
            Vector3 endPoint = GetEndPoint(powID, difSpawn);

            //Get aiming vector for when throwing the powerup
            Vector3 direction = endPoint - startPoint;
            direction.Normalize();
            if (difSpawn)
            {
                direction.y = 0.75f;
            }
            else
            {
                direction.y = 0.5f;
            }

            //Get speed to throw the powerup with
            float speed = GetSpeed(difSpawn);

            GameObject spawnedPU;
            //Insantiate the powerup gameobject, get its script, and call the set direction script to get it thrown
            if (!RoundManager.Instance.screwMode)
                spawnedPU = Instantiate(powerup, startPoint, Quaternion.identity);
            else
                spawnedPU = Instantiate(screwPickup, startPoint, Quaternion.identity);

            PowerUpPickup scriptPU = spawnedPU.GetComponent<PowerUpPickup>();
            scriptPU.SetDirection(direction, speed);

            //Add new powerup pickup to the powerups list

            powerups.Add(spawnedPU);

        }
    }

    //Randomly picks a powerup to spawn, passes in a reference of the powID to use it later
    private GameObject ChoosePowerup(ref PowerupType pow)
    {
        //initializing powerup object
        GameObject powerup;

        //get random number 0-3
        int randNum = Random.Range(0, powerupPrefabs.Count);

        //powerupID is equal to that random number, translated into PowerupType
        pow = (PowerupType)randNum;

        //Set powerup gameobject to specific powerup prefab based on powerupID
        powerup = powerupPrefabs[randNum];

        //return the powerup gameobject
        return powerup; 
    }

    //Gets the starting point to throw the powerup from
    private Vector3 GetStartPoint(PowerupType pow, bool difSpawn)
    {
        //initialize starting point vector
        Vector3 startPoint = new Vector3();

        //if its spawning from a different position than normal
        if (difSpawn)
        {
            //if its the popcorn powerup
            if (pow == PowerupType.Popcorn)
            {
                //spawn from specific popcornSpawn position
                startPoint = popcornSpawnPos;
            }
        }
        //else it spawns the normal full map way
        else
        {
            //get a random angle between 1 and 360
            int angle = Random.Range(1, 361);

            float radAngle = angle * Mathf.Deg2Rad;

            //gets x and z position of the starting point by using a large circle with the radius equal to the radius variable
            float z = Mathf.Sin(radAngle) * radius;
            float x = Mathf.Cos(radAngle) * radius;

            //set those x and z values to the startpoint vector
            startPoint = mapCenter + new Vector3(x, 0f, z);
        }

        //return the startPoint vector
        return startPoint;
    }

    //Gets the endPoint position that the powerup is going to aim towards
    private Vector3 GetEndPoint(PowerupType pow, bool difSpawn)
    {
        //initializes the endPoint vector
        Vector3 endPoint = new Vector3();

        //if its spawning from a different spawning position
        if (difSpawn)
        {
            //if powerup is the popcorn powerup
            if (pow == PowerupType.Popcorn)
            {
                //get random angle between 1 and 360
                int angle = Random.Range(minAngle, maxAngle);

                float radAngle = angle * Mathf.Deg2Rad;

                //gets x and z positions on a circle thats got a radius of the variable radius
                float z = Mathf.Sin(radAngle);
                float x = Mathf.Cos(radAngle);

                //endPoint is the startPoint + those new x and z positions
                endPoint = popcornSpawnPos + new Vector3(x, 0f, z);
            }
        }
        //else if the powerup is spawning normally
        else
        {
            //endPoint is just the origin
            endPoint = mapCenter;
        }

        //return the endPoint vector
        return endPoint;
    }

    //Calculates a random speed value
    private float GetSpeed(bool difSpawn)
    {
        //initialize speed float
        float speed;

        //if its spawning from a different spawn point
        if (difSpawn)
        {
            //speed is set to 4.2f
            speed = 5.2f;
        }
        //else its spawning normally
        else
        {
            //speed is set to a specific range that is proportional to the radius of the circle
            speed = Random.Range(radius * 0.3f, radius * 0.65f);
        }

        //return the speed float
        return speed;
    }

    //gets the popcorn machine from the scene
    public void GetPopcornMachine()
    {
        popcornSpawn = GameObject.FindObjectOfType<PopcornMachine>().GetComponent<PopcornMachine>();

        if (popcornSpawn != null)
        {
            popcornSpawnPos = popcornSpawn.transform.position + new Vector3(0f, 6f, 0f);
            minAngle = popcornSpawn.minAngle;
            maxAngle = popcornSpawn.maxAngle;

            while (maxAngle < minAngle)
            {
                maxAngle += 360;
            }
            maxAngle++;
        }
    }
}
