using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInputInformation : MonoBehaviour
{
    private static PlayerInputInformation instance;
    public static PlayerInputInformation Instance
    {
        get { return instance; }
    }

    public Dictionary<int, InputDevice> playerInfo = new Dictionary<int, InputDevice>();
    public List<float> playerScores = new List<float>();
    public List<Dictionary<WorldData.statsType, float>> playerStats = new List<Dictionary<WorldData.statsType, float>>();
    public List<int> playerHats = new List<int>();
    public List<int> playerSkins = new List<int>();

    PlayerInputManager pim;

    public int totalPlayers = 0;

    public bool canJoin;

    //System.Action<UnityEngine.InputSystem.PlayerInput>
    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }
    private void Start()
    {
        playerHats.Add(0);
        playerHats.Add(0);
        playerHats.Add(0);
        playerHats.Add(0);

        playerSkins.Add(0);
        playerSkins.Add(0);
        playerSkins.Add(0);
        playerSkins.Add(0);



    }
    public void GetPlayerInfo(PlayerInput newPlayer)
    {
        if (newPlayer.devices.Count >= 1 && canJoin)
        {
            playerInfo.Add(newPlayer.playerIndex, newPlayer.devices[0]);
        }
    }

    public void DestroyAllPlayers()
    {
        PlayerController[] playerList = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (PlayerController player in playerList)
        {
            DestroyImmediate(player.gameObject);
        }
    }

    public void UpdateHat(int index, int hat)
    {
        playerHats[index] = hat;
    }

    public void UpdateSkin(int index, int skin)
    {
        playerSkins[index] = skin;
    }

    public void UpdateScores(List<float> scores)
    {
        playerScores = scores;
    }

    public void UpdateStats(List<Dictionary<WorldData.statsType, float>> stats)
    {
        playerStats = stats;
    }

    public int GetTotalPlayers()
    {
        return totalPlayers;
    }
}
