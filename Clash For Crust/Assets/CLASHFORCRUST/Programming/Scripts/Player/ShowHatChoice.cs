using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHatChoice : MonoBehaviour
{
    private int playerNum;
    public int PlayerNum { get { return playerNum; } }

    PlayerController pc;
    SkinnedMeshRenderer rend;

    private Transform hatSpawner;
    private GameObject hat;
    private Material skin;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        hatSpawner = GetComponentInChildren<HatSpawner>().gameObject.transform;

        if (pc)
        {
            playerNum = pc.playerNum;
        }
        else
        {
            playerNum = int.Parse(name.Substring(1, 1));
            playerNum -= 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {


        int hatChoice, skinChoice;

        if (RoundManager.Instance)
        {
            if (!RoundManager.Instance.joinOnButton)
            {
                hatChoice = PlayerInputInformation.Instance.playerHats[playerNum];
                skinChoice = PlayerInputInformation.Instance.playerSkins[playerNum];
            }
            else
            {
                hatChoice = 0;
                skinChoice = 0;
            }
        }
        else
        {
            hatChoice = PlayerInputInformation.Instance.playerHats[playerNum];
            skinChoice = PlayerInputInformation.Instance.playerSkins[playerNum];
        }

        hat = Instantiate(HatsAndSkins.Instance.Hats[hatChoice], hatSpawner.transform);

        skin = HatsAndSkins.Instance.Skins[skinChoice];
        rend.material = skin;
        if (pc)
            pc.currentMat = skin;
    }
}
