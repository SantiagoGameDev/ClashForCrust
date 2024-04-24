//Author: Branson Vernon
//Date: 01/30/24
//Purpose: Does all of the logic for a game round

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEditor;

public class RoundManager : MonoBehaviour
{
    private static RoundManager instance;

    public static RoundManager Instance
    {
        get { return instance; }
    }

    public List<PlayerController> players = new List<PlayerController>();

    public GameObject goldenCrust;
    public GameObject goldenScrew;
    public Transform crustSpawn;

    [SerializeField] private TMP_Text winnerBox;

    [SerializeField]
    private float startTime, gameTime;

    public float GameTime
    { get { return gameTime; } set { gameTime = value; } }

    public float StartTime
    { get { return startTime; } }

    //if game is over
    private bool gameStart;

    public bool GameStart
    {
        get { return gameStart; }
    }

    private bool gameOver;

    public bool GameOver
    {
        get { return gameOver; }
    }

    //if powerups are active
    [SerializeField]
    private bool powerOn = true;

    public bool PowerOn
    { get { return powerOn; } }

    //Powerup spawn frequency and spawn timer
    private float powerSpawnFrequency = 5;

    public float PowerSpawnFrequency
    { get { return powerSpawnFrequency; } }

    //if on beachfront map
    private bool wavesOn;

    public bool WavesOn
    { get { return wavesOn; } }

    //Wave frequency and wave timer
    private bool waveFrequency, waveTimer;

    private bool timeAlmostUp;

    //Golden Crust starting value
    [SerializeField]
    private bool crustOn = true;

    public bool CrustOn
    { get { return crustOn; } }

    private int crustValue = 1;
    private int currentCrustValue;
    public int CurrentCrustValue
    { get { return currentCrustValue; } }

    [SerializeField]
    private bool fryOn = true;

    public bool FryOn
    { get { return fryOn; } }

    private float frySpawnFrequency = 5f;
    public float FrySpawnFrequency
    { get { return frySpawnFrequency; } }

    //who's currently holding the crust, set to zero when no one is
    private int holdingCrust;

    public int HoldingCrust
    { get { return holdingCrust; } }

    [SerializeField]
    private int maxHealth = 5;

    public int MaxHealth
    { get { return maxHealth; } }

    //Crust value multipliers at 40, 20, and 10 percent of the timer left
    [SerializeField]
    private int crustPercent40 = 2;

    [SerializeField]
    private int crustPercent20 = 3;

    [SerializeField]
    private int crustPercent10 = 4;

    [SerializeField]
    private MapController mapController;

    public List<Transform> playerSpawns = new List<Transform>();
    private bool gottenSpawners;

    private GameObject currentLayout;

    public int playersRequiredToStart = 4;

    public bool screwMode = false;

    [SerializeField]
    private PlayerInputManager pim;

    public bool joinOnButton = false;

    [SerializeField]
    private bool freezeTimer = false;

    public bool hideHud = false;

    private bool startGOC = false;

    [SerializeField] private GameObject boat;

    private bool crustSpawnedIn;

    public bool introLineDone;

    public Transform middle;

    //instance = this
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < 4; i++)
        {
            players.Add(null);
        }

        StartCoroutine(GetPlayers());
        timeAlmostUp = false;
        introLineDone = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitializeGameValues();

        gameStart = true;
        gameTime = startTime;
        gameOver = false;
        crustSpawnedIn = false;

        holdingCrust = -1;

        mapController.ChangeLayout();

        if (!joinOnButton)
        {
            if (PlayerInputInformation.Instance != null)
            {
                playersRequiredToStart = PlayerInputInformation.Instance.totalPlayers;

                for (int i = 0; i < playersRequiredToStart; i++)
                {
                    PlayerInput player = pim.JoinPlayer(i, -1, null, PlayerInputInformation.Instance.playerInfo[i]);
                    player.name = "Player" + (i + 1) + "Seagull";
                    player.transform.parent = pim.transform;
                    //if (boat == null)
                    //{
                    //    player.transform.parent = pim.transform;
                    //}
                    //else
                    //{
                    //    player.transform.parent = boat.transform;
                    //}
                }
            }
            WorldData.Instance.InitializeEverything();
        }
        else
        {
            pim.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameStart && gottenSpawners)
        {
            PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();

            foreach (PlayerController player in allPlayers)
            {
                if (player.gameObject.tag == "Player1")
                {
                    players[0] = player;
                    players[0].gameObject.transform.position = playerSpawns[0].transform.position;
                }

                if (player.gameObject.tag == "Player2")
                {
                    players[1] = player;
                    players[1].gameObject.transform.position = playerSpawns[1].transform.position;
                }

                if (player.gameObject.tag == "Player3")
                {
                    players[2] = player;
                    players[2].gameObject.transform.position = playerSpawns[2].transform.position;
                }

                if (player.gameObject.tag == "Player4")
                {
                    players[3] = player;
                    players[3].gameObject.transform.position = playerSpawns[3].transform.position;
                }

                if (crustOn)
                {
                    if (!crustSpawnedIn)
                    {
                        SpawnCrust();
                        crustSpawnedIn = true;
                        WorldData.Instance.CurrentCamera.transform.position = GoldenCrust.Instance.transform.position + new Vector3(-1.53f, 10.37f, -5.36f);
                    }
                }
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject[] crusts = GameObject.FindGameObjectsWithTag("CrustPickup");

            foreach (GameObject crust in crusts)
            {
                Destroy(crust);
            }

            Instantiate(GoldenCrust, crustSpawn.position, Quaternion.identity);
        }*/

        //subtract game time
        if (!gameOver)
        {
            if (!freezeTimer && !gameStart)
            {
                gameTime -= Time.deltaTime;
            }

            currentCrustValue = crustValue;

            //at 10% of the timer left, crust is worth crustPercent10
            if (gameTime <= startTime * 0.1)
            {
                currentCrustValue = crustValue * crustPercent10;
            }
            //at 20% of the timer left, crust is worth crustPercent20
            else if (gameTime <= startTime * 0.2)
            {
                if (timeAlmostUp == false)
                {
                    timeAlmostUp = true;
                    AudioManager.Instance.PlayAudio(AudioManager.AudioType.TimeTickin, true);
                }
                currentCrustValue = crustValue * crustPercent20;
            }
            //at 40% of the timer left, crust is worth crustPercent40
            else if (gameTime <= startTime * 0.4)
            {
                currentCrustValue += crustValue * crustPercent40;
            }

            //if game time is less than zero, set it to zero and end the game
            if (gameTime <= 0f)
            {
                gameTime = 0f;
                gameOver = true;
            }
        }

        if (gameOver && !startGOC)
        {
            StartCoroutine(GameEnded());
        }
    }

    private IEnumerator GameEnded()
    {
        startGOC = true;
        for (int i = 0; i < playersRequiredToStart; i++)
        {
            players[i].controllable = false;
        }

        if (holdingCrust != -1)
        {
            players[holdingCrust].RevertBack();
        }

        if (GoldenCrust.Instance)
        {
            Destroy(GoldenCrust.Instance);
        }

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.MatchOver, true);
        yield return new WaitForSeconds(2f);
        CreateScoreCopies();
        CreateStatCopies();
        SceneControl.Instance.LoadSceneFromName("AwardShow");
        gameOver = false;
    }

    //sets who is currently holding the crust
    public void SetHoldingCrust(int playerNum)
    {
        holdingCrust = playerNum;
    }

    //Will take stuff in from the rules selection menu
    public void InitializeGameValues()
    {
        if (RuleChoiceInfo.Instance)
        {
            startTime = RuleChoiceInfo.Instance.gameTime;
            maxHealth = RuleChoiceInfo.Instance.maxHealth;
            crustOn = RuleChoiceInfo.Instance.crustOn;
            crustValue = RuleChoiceInfo.Instance.crustValue;
            powerOn = RuleChoiceInfo.Instance.powerupsOn;
            powerSpawnFrequency = RuleChoiceInfo.Instance.powerupSpawnRate;
            fryOn = RuleChoiceInfo.Instance.fryOn;
            frySpawnFrequency = RuleChoiceInfo.Instance.frySpawnRate;
            screwMode = RuleChoiceInfo.Instance.screwMode;
        }
    }

    private IEnumerator GetPlayers()
    {
        int playerCount = 0;

        while (playerCount < playersRequiredToStart)
        {
            playerCount = 0;

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    playerCount++;
                }
            }
        }

        pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

        StartCoroutine(HudManager.Instance.IntroLine());
    }

    public void StartMatch()
    {
        gameTime = startTime;
        gameStart = false;

        PowerupSpawner.Instance.SetSpawnFrequency();
        PowerupSpawner.Instance.SetFryFrequency();

        PowerupSpawner.Instance.canSpawn = true;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                players[i].StartGame();
            }
        }

        HudManager.Instance.StartTimer();
    }

    public void SpawnCrust()
    {
        GameObject gc;
        if (!screwMode)
            gc = Instantiate(goldenCrust, crustSpawn.position, Quaternion.identity);
        else
            gc = Instantiate(goldenScrew, crustSpawn.position, Quaternion.identity);

        gc.transform.parent = transform;
    }

    public void GetSpawners()
    {
        int spawners = 0;

        crustSpawn = GameObject.FindWithTag("CrustSpawn").transform;
        if (crustSpawn != null)
        {
            spawners++;
        }

        playerSpawns.Add(GameObject.FindWithTag("P1Spawn").transform);
        if (playerSpawns[0] != null)
        {
            spawners++;
        }

        playerSpawns.Add(GameObject.FindWithTag("P2Spawn").transform);
        if (playerSpawns[1] != null)
        {
            spawners++;
        }

        playerSpawns.Add(GameObject.FindWithTag("P3Spawn").transform);
        if (playerSpawns[2] != null)
        {
            spawners++;
        }

        playerSpawns.Add(GameObject.FindWithTag("P4Spawn").transform);
        if (playerSpawns[3] != null)
        {
            spawners++;
        }

        if (spawners == 5)
        {
            PowerupSpawner.Instance.GetPopcornMachine();
            gottenSpawners = true;
        }
    }

    private void CreateScoreCopies()
    {
        //Create a copy of the current scores and pass them to the input information so they can be transferred to the Award Scene
        List<float> scoreCopies = new List<float>();

        //Game over here
        for (int i = 0; i < players.Count; i++)
        {
            scoreCopies.Add(WorldData.Instance.players[i][WorldData.statsType.calorieCount]);
        }

        PlayerInputInformation.Instance.UpdateScores(scoreCopies);
    }

    private void CreateStatCopies()
    {
        List<Dictionary<WorldData.statsType, float>> playerStats = new List<Dictionary<WorldData.statsType, float>>(WorldData.Instance.players);

        foreach (var stats in WorldData.Instance.players)
        {
            //Debug.Log(stats[WorldData.statsType.movement]);
            playerStats.Add(stats);
        }

        PlayerInputInformation.Instance.UpdateStats(playerStats);
    }

    public IEnumerator CameraZoomOut()
    {
        Vector3 normalPos = WorldData.Instance.startCameraPos;
        Vector3 startPos = WorldData.Instance.CurrentCamera.transform.position;
        float scale = 0f;

        Vector3 difference = normalPos - startPos;

        //yield return new WaitForSeconds(1f);

        while (scale < 1f)
        {
            scale += Time.deltaTime * 0.5f;

            if (scale > 1f)
            {
                scale = 1f;
            }

            float easedScale = EaseIn(scale);

            Vector3 newPos = startPos + (difference * easedScale);

            WorldData.Instance.CurrentCamera.transform.position = newPos;

            yield return null;
        }


        yield return null;
    }

    //private IEnumerator IntroLine()
    //{
    //    AudioManager.Instance.PlayAudio(AudioManager.AudioType.BattleIntro, true);

    //    yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
    //}

    private float EaseIn(float inValue)
    {
        return -(Mathf.Cos(Mathf.PI * inValue) - 1f) / 2f;
    }

}