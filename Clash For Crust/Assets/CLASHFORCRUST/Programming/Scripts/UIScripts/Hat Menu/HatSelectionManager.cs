using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HatSelectionManager : MonoBehaviour
{
    private static HatSelectionManager instance;
    public static HatSelectionManager Instance { get { return instance; } }

    private List<HatSelection> hatSelections = new List<HatSelection>();
    public List<HatSelection> HatSelections { get { return hatSelections; } }

    [SerializeField] List<GameObject> playerCanvases = new List<GameObject>();
    [SerializeField] List<GameObject> playerPodiums = new List<GameObject>();
    [SerializeField] List<GameObject> pressAToJoins = new List<GameObject>();  
    [SerializeField] List<GameObject> playerHatWheels = new List<GameObject>();
    [SerializeField] List<GameObject> playerSkinWheels = new List<GameObject>();

    [SerializeField] bool allSelected;
    [SerializeField] GameObject allSelectedObj;

    [SerializeField] HatPIM hatPIM;

    private int playersJoined = 0;

    private void Start()
    {
        instance = this;

        allSelected = false;

        HatSelection[] hatSelectionsArray = FindObjectsOfType<HatSelection>();

        for (int i = 0; i < 4; i++)
        {
            hatSelections.Add(null);
        }

        foreach (HatSelection hs in hatSelectionsArray)
        {
            hatSelections[hs.PlayerID] = hs;
        }

        for (int i = 0; i < hatSelections.Count; i++)
        {
            hatSelections[i].gameObject.SetActive(false);
            playerCanvases[i].gameObject.SetActive(false);
            playerPodiums[i].gameObject.SetActive(false);
            playerHatWheels[i].gameObject.SetActive(false);
            playerSkinWheels[i].gameObject.SetActive(false);
        }

        hatPIM.StartJoining();
        PlayerInputInformation.Instance.canJoin = true;
        PlayerInputInformation.Instance.playerInfo = new Dictionary<int, UnityEngine.InputSystem.InputDevice>();

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.CustomizationMenu, true);
        

    }

    //private void Update()
    //{
    //    for (int i = 0; i < PlayerInputInformation.Instance.totalPlayers; i++)
    //    {
    //        MakeGullVisible(i);
    //    }
    //}

    public void CheckForReady()
    {
        int readied = 0;

        //Debug.Log("Checking for ready");
        foreach (HatSelection hats in hatSelections)
        {
            if (hats.LockedIn())
            {
                readied++;
                //Debug.Log("Not ready");
            } 
        }

        //Debug.Log("THis many people ready: " + readied);
        //Debug.Log("This many players joined: " + playersJoined);
        //get rid of playersJoined > 1 if you want to use 1 guy
        if (readied == playersJoined /*&& playersJoined > 1*/)
        {
            //Debug.Log("All Ready");
            allSelected = true;
            allSelectedObj.SetActive(true);
            PlayerInputInformation.Instance.totalPlayers = playersJoined;
            //PlayerInputInformation.Instance.canJoin = false;

        }
        else
        {
            allSelected = false;
            allSelectedObj.SetActive(false);
        }
    }

    public void MakeGullVisible(int index)
    {
        hatSelections[index].gameObject.SetActive(true);
        playerCanvases[index].SetActive(true);
        playerPodiums[index].gameObject.SetActive(true);
        pressAToJoins[index].gameObject.SetActive(false);
        playerHatWheels[index].gameObject.SetActive(true);
        playerSkinWheels[index].gameObject.SetActive(true);

        playersJoined++;

        CheckForReady();
    }

    public void OnStartPress()
    {
        if(allSelected)
            SceneControl.Instance.LoadSceneFromName("RulesMenu");
    }
}
