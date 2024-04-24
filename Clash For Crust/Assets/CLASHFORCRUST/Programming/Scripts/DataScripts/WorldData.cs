//Author: Branson Vernon
//Date Created: 01/11/2024
//Purpose: Keeps track of all the information of a current game

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    //The one instance of the world data
    private static WorldData instance;

    public static WorldData Instance
    {
        get { return instance; }

        set { instance = value; }
    }

    //enumerated type for all the different stats
    public enum statsType
    { calorieCount = 0, knockOuts = 1, friesEaten = 2, powerups = 3, knockedOut = 4, totalCrustTime = 5, totalPeppers = 6, fireDamage = 7, totalPopcorn = 8, totalFireworks = 9, totalDonuts = 10, spinHits = 11, smashHits = 12, shotUses = 13, movement = 14, totalDamage = 15 }

    public const int NUM_STATS = 16;

    //lost of dictionaries of stat information for each player
    public List<Dictionary<statsType, float>> players = new List<Dictionary<statsType, float>>();

    //numer of players
    public int numPlayers;

    //Max Player Health
    private int maxHealth;

    public int MaxHealth
    { get { return maxHealth; } }

    //if game is over
    public bool gameOver = false;

    //max powerups and current number of powerups in existence
    public int totalPowerups;

    private int maxPowerups;

    public int MaxPowerups
    { get { return maxPowerups; } }

    //The colors that each player is assigned
    [SerializeField]
    private Color[] playerColors = new Color[4];

    [SerializeField] private Camera currentCamera;

    public Camera CurrentCamera
    { get { return currentCamera; }
      set { currentCamera = value; }
    }

    public Vector3 startCameraPos;

    //On Frame 1
    private void Start()
    {
        //When events manager exists, subscribe to stat update event
        //EventsManager.Instance.StatUpdate += UpdateStats;

        if (currentCamera == null)
        {
            currentCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        startCameraPos = currentCamera.transform.position;
    }

    //returns the current game time
    public float GetGameTime()
    {
        return RoundManager.Instance.GameTime;
    }

    public float GetStartTime()
    {
        return RoundManager.Instance.StartTime;
    }

    //returns if the game is over or not
    public bool IsGameOver()
    {
        return RoundManager.Instance.GameOver;
    }

    //returns if the powerups are on
    public bool ArePowerUpsOn()
    {
        return RoundManager.Instance.PowerOn;
    }

    //returns the spawn frequency of powerups
    public float GetPowerSpawnFrequency()
    {
        return RoundManager.Instance.PowerSpawnFrequency;
    }

    //returns the current value of the golden crust
    public int GetCrustValue()
    {
        return RoundManager.Instance.CurrentCrustValue;
    }

    public int WhoHasCrust()
    {
        return RoundManager.Instance.HoldingCrust;
    }

    // instance is this
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //Initiates all the stats for a player
    private void InitiatePlayerStats(Dictionary<statsType, float> player)
    {
        //for all stats
        for (int i = 0; i < NUM_STATS; i++)
        {
            //initialize statsType stat to 0
            player.Add((statsType)i, 0);
        }
    }

    //Calls the initiatePlayerStats script for all players
    private void InitiateAllPlayerStats()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            InitiatePlayerStats(players[i]);
        }
    }

    //call this whenever a stat is updated
    public void UpdateStats(int playerNum, statsType stat, float valueToAdd)
    {
        if (!RoundManager.Instance.GameStart)
        {
            if (players[playerNum] != null)
            {
                float newValue;
                if (players[playerNum].TryGetValue(stat, out newValue))
                {
                    players[playerNum][stat] = newValue + valueToAdd;
                }
            }
        }
    }

    //to be called when the game starts, initializes all world data values
    public void InitializeEverything()
    {
        numPlayers = RoundManager.Instance.players.Count;
        //set player list to the correct number of players
        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Dictionary<statsType, float>());
        }

        InitiateAllPlayerStats();
    }

    public GameObject GetPlayerWithHighestScore() // used to zoom camera at end of game, NOT for awards
    {
        float highestCalories = 0;
        GameObject fattestSeagull = null;

        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i][statsType.calorieCount] > highestCalories)
            {
                highestCalories = players[i][statsType.calorieCount];
                fattestSeagull = RoundManager.Instance.players[i].gameObject;
            }
        }

        return fattestSeagull;
    }
}