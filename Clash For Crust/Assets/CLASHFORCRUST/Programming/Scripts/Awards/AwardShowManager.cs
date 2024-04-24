using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AwardShowManager : MonoBehaviour
{
    private static AwardShowManager instance;
    public static AwardShowManager Instance { get { return instance; } }

    [SerializeField] public AwardList awardList;

    [SerializeField] private float audioDelay;
    [SerializeField] private float animDelay;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drumRollAudio;
    [SerializeField] private AudioClip awardSpawnAudio;

    //Scores
    [SerializeField] private List<float> scores;
    private float totalScore;
    public float scoreMult;
    private bool firstScoreCheck;

    //Seagull models
    private List<GameObject> seagulls;
    [SerializeField] private List<GameObject> podiums;
    [SerializeField] private List<GameObject> scoreBoxes;

    //Texts
    [SerializeField] private List<TMP_Text> scoreTxts = new List<TMP_Text>();
    [SerializeField] private TMP_Text awardNameTxt;
    [SerializeField] private TMP_Text winnerTxt;

    //Panel
    [SerializeField] GameObject btnPanel;

    //Award Spawn Points and Images
    [SerializeField] private List<Transform> awardSpawns = new List<Transform>();
    private List<Image> awardImages = new List<Image>();
    bool awardDissolve = false;
    float dissolveDuration = 3f;

    [SerializeField] private List<GameObject> spotlights = new List<GameObject>();

    [SerializeField] private GameObject awardFry;
    [SerializeField] private List<Transform> frySpawner;
    [SerializeField] private AudioSource eatingSource;

    [SerializeField] private Image mainAwardImage;

    [SerializeField] private GameObject winnerBanner;
    [SerializeField] private TMP_Text multiplierText;

    public bool awardsDone = false;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private void Start()
    {
        PlayerInputInformation.Instance.DestroyAllPlayers();
        AudioManager.Instance.PirateModeToggle(false);

        scores = PlayerInputInformation.Instance.playerScores;
        firstScoreCheck = false;

        seagulls = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            seagulls.Add(null);
        }

        ShowHatChoice[] gulls = FindObjectsOfType<ShowHatChoice>();
        foreach (ShowHatChoice gull in gulls) 
        {
            seagulls[gull.PlayerNum] = gull.gameObject;
            seagulls[gull.PlayerNum].SetActive(false);
        }

        InitGulls();
        InitScores();

        mainAwardImage.enabled = false;
        scoreMult = 1 + (int)(totalScore * 0.0125f);
        multiplierText.text = "x" + scoreMult;

        foreach (GameObject scorebox in scoreBoxes)
        {
            scorebox.SetActive(false);
        }

        for (int i = 0; i < awardSpawns.Count; i++)
        {
            awardImages.Add(awardSpawns[i].gameObject.GetComponentInChildren<Image>());
            awardImages[i].color = Color.clear;
        }

        if (RuleChoiceInfo.Instance.awardsOn)
        {
            AwardShowStart();
        }
        else
        {
            StartCoroutine(AnnounceWinner());
        }
    }

    private void InitGulls()
    {
        seagulls[0].SetActive(true);
        podiums[0].SetActive(true);
        scoreBoxes[0].GetComponent<Image>().enabled = true;
        scoreBoxes[0].GetComponentInChildren<TMP_Text>().enabled = true;
        
        if (PlayerInputInformation.Instance.totalPlayers >= 2)
        {
            seagulls[1].SetActive(true);
            podiums[1].SetActive(true);
            scoreBoxes[1].GetComponent<Image>().enabled = true;
            scoreBoxes[1].GetComponentInChildren<TMP_Text>().enabled = true;
        }


        if (PlayerInputInformation.Instance.totalPlayers >= 3)
        {
            seagulls[2].SetActive(true);
            podiums[2].SetActive(true);
            scoreBoxes[2].GetComponent<Image>().enabled = true;
            scoreBoxes[2].GetComponentInChildren<TMP_Text>().enabled = true;
        }

        if (PlayerInputInformation.Instance.totalPlayers == 4)
        {
            seagulls[3].SetActive(true);
            podiums[3].SetActive(true);
            scoreBoxes[3].GetComponent<Image>().enabled = true;
            scoreBoxes[3].GetComponentInChildren<TMP_Text>().enabled = true;
        }

        foreach (GameObject gull in seagulls)
        {
            if(gull)
                gull.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Shades", 2.0f);
        }

    }

    private void InitScores()
    {
        for (int i = 0; i < WorldData.Instance.numPlayers; i++)
        {
            scoreTxts[i].text = scores[i].ToString();

            if(!firstScoreCheck)
                totalScore += scores[i];
        }

        firstScoreCheck = true;
    }

    private void ShowButtons()
    {
        btnPanel.SetActive(true);

        awardNameTxt.text = "";

        awardsDone = true;
    }

    private IEnumerator AnnounceWinner()
    {
        float maxScore = -1;
        int winnerIndex = -1;
        int secondIndex = -1, thirdIndex = -1, fourthIndex = -1;
        float secondScore = -1, thirdScore = -1, fourthScore = -1;

        for (int i = 0; i < PlayerInputInformation.Instance.totalPlayers; i++)
        {
            if (PlayerInputInformation.Instance.playerScores[i] > maxScore)
            {
                if (PlayerInputInformation.Instance.totalPlayers > 3)
                {
                    fourthIndex = thirdIndex;
                    fourthScore = thirdScore;
                }
                if (PlayerInputInformation.Instance.totalPlayers > 2)
                {
                    thirdIndex = secondIndex;
                    thirdScore = secondScore;
                }
                if (PlayerInputInformation.Instance.totalPlayers > 1)
                {
                    secondIndex = winnerIndex;
                    secondScore = maxScore;
                }

                winnerIndex = i;
                maxScore = PlayerInputInformation.Instance.playerScores[i];
            }
            else if (PlayerInputInformation.Instance.playerScores[i] > secondScore)
            {
                if (PlayerInputInformation.Instance.totalPlayers > 3)
                {
                    fourthIndex = thirdIndex;
                    fourthScore = thirdScore;
                }
                if (PlayerInputInformation.Instance.totalPlayers > 2)
                {
                    thirdIndex = secondIndex;
                    thirdScore = secondScore;
                }

                if (PlayerInputInformation.Instance.totalPlayers > 1)
                {
                    secondIndex = i;
                    secondScore = PlayerInputInformation.Instance.playerScores[i];
                }
            }
            else if (PlayerInputInformation.Instance.playerScores[i] > thirdScore)
            {
                if (PlayerInputInformation.Instance.totalPlayers > 3)
                {
                    fourthIndex = thirdIndex;
                    fourthScore = thirdScore;
                }

                if (PlayerInputInformation.Instance.totalPlayers > 2)
                {
                    thirdIndex = i;
                    thirdScore = PlayerInputInformation.Instance.playerScores[i];
                }
            }
            else
            {
                if (PlayerInputInformation.Instance.totalPlayers > 3)
                {
                    fourthIndex = i;
                    fourthScore = PlayerInputInformation.Instance.playerScores[i];
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (i < PlayerInputInformation.Instance.totalPlayers)
            {
                float scale = 0f;

                if (i == winnerIndex)
                {
                    scale = 90f;
                }
                else if (i == secondIndex)
                {
                    scale = 75f;
                }
                else if (i == thirdIndex)
                {
                    scale = 60f;
                }
                else if (i == fourthIndex)
                {
                    scale = 45f;
                }

                StartCoroutine(ResizeGull(scale, i));
            }
        }

        audioSource.clip = drumRollAudio;
        audioSource.Play();

        awardNameTxt.text = "But who Won?";
        mainAwardImage.enabled = false;

        yield return new WaitForSeconds(1.7f);

        mainAwardImage.enabled = false;
        winnerTxt.gameObject.SetActive(true);

        TurnOnSpotlight(winnerIndex);

        winnerBanner.SetActive(true);

        for (int i = 0; i < PlayerInputInformation.Instance.GetTotalPlayers(); i++)
        {
            if (i < PlayerInputInformation.Instance.GetTotalPlayers())
            {
                scoreBoxes[i].SetActive(true);
                scoreTxts[i].text = scores[i].ToString();
            }
        }

        switch (winnerIndex)
        {
            case 0:
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.BlueWins, true);
                winnerTxt.text = "Blue Wins!";
                //winnerTxt.color = Color.blue;
                break;

            case 1:
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.RedWins, true);
                winnerTxt.text = "Red Wins!";
                //winnerTxt.color = Color.red;
                break;

            case 2:
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.GreenWins, true);
                winnerTxt.text = "Green Wins!";
                //winnerTxt.color = Color.green;
                break;

            case 3:
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.YellowWins, true);
                winnerTxt.text = "Yellow Wins!";
                //winnerTxt.color = Color.yellow;
                break;
            default:
                break;
        }

        seagulls[winnerIndex].GetComponent<Animator>().SetInteger("IdleVar", 1);

        if (secondIndex != -1)
        {
            if (PlayerInputInformation.Instance.totalPlayers > 2)
                seagulls[secondIndex].GetComponent<Animator>().SetInteger("IdleVar", 2);
            else if (PlayerInputInformation.Instance.totalPlayers == 2)
            {
                int random = Random.Range(1, 3);
                seagulls[secondIndex].GetComponent<Animator>().SetInteger("Stunned", random);
            }
        }
        if (thirdIndex != -1)
        {
            if (PlayerInputInformation.Instance.totalPlayers == 4)
                seagulls[thirdIndex].GetComponent<Animator>().SetInteger("IdleVar", 2);
            else if (PlayerInputInformation.Instance.totalPlayers == 3)
            {
                int random = Random.Range(1, 3);
                seagulls[thirdIndex].GetComponent<Animator>().SetInteger("Stunned", random);
            }
        }
        if (fourthIndex != -1) 
        {
            int random = Random.Range(1, 3);
            seagulls[fourthIndex].GetComponent<Animator>().SetInteger("Stunned", random);
        }

        awardNameTxt.text = "";

        StartCoroutine(WaitToShowButtons());
    }

    public void AwardShowStart()
    {
        StartCoroutine(AwardSelection());
    }

    private void TurnOnSpotlight(int player)
    {
        spotlights[player].gameObject.SetActive(true);
        seagulls[player].GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Shades", 0.05f);
    }

    private void TurnOffSpotlight(int player)
    {
        spotlights[player].gameObject.SetActive(false);
        seagulls[player].GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_Shades", 2.0f);
    }

    private IEnumerator AwardSelection()
    {
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.AwardIntro, true);

        foreach (Award award in awardList.selectedAwards)
        {
            while (AudioManager.Instance.IsPlaying())
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
            //yield return new WaitForSeconds(audioDelay);

            awardNameTxt.text = award.name;
            mainAwardImage.enabled = true;
            mainAwardImage.sprite = award.awardImg;
            AudioManager.Instance.PlayAudio(award.audioType, true);
            //while (AudioManager.Instance.IsPlaying())
            //{
                //yield return null;
            //}

            yield return new WaitForSeconds(1.1f);

            //Drumroll please...
            audioSource.clip = drumRollAudio;
            audioSource.Play();

            yield return new WaitForSeconds(1.7f);

            List<int> winnerIDs = awardList.CompareStats(award);
            foreach (int winner in winnerIDs)
            {
                TurnOnSpotlight(winner);
            }

            //yield return new WaitForSeconds(animDelay);

            //Setting the award's image and starting the dissolve process

            //audioSource.clip = awardSpawnAudio;
            //audioSource.Play();

            for (int i = 0; i < PlayerInputInformation.Instance.totalPlayers; i++)
            {
                bool won = false;
                foreach(int winner in winnerIDs)
                {
                    if (i == winner)
                    {
                        won = true;
                    }
                }

                if (!won)
                {
                    int random = Random.Range(1, 3);
                    seagulls[i].GetComponent<Animator>().SetInteger("Stunned", random);
                }
                else
                {
                    seagulls[i].GetComponent<Animator>().SetInteger("IdleVar", 1);
                }
            }

            foreach (int winner in winnerIDs)
            {
                //awardImages[winner].sprite = award.awardImg;
                //awardImages[winner].color = Color.white;
                //StartCoroutine(DissolveAward(awardImages[winner]));

                StartCoroutine(SpawnFries(winner, award.calorieCountBonus));
            }
            

            yield return new WaitForSeconds(2.5f);

            for (int i = 0; i < PlayerInputInformation.Instance.totalPlayers; i++)
            {
                foreach (int winner in winnerIDs)
                {
                    if (i != winner)
                    {
                        seagulls[i].GetComponent<Animator>().SetInteger("Stunned", 0);
                    }
                    else
                    {
                        seagulls[i].GetComponent<Animator>().SetInteger("IdleVar", 0);
                    }
                }
            }

            foreach (int winner in winnerIDs)
            {
                TurnOffSpotlight(winner);

                //scores[winner] += award.calorieCountBonus * scoreMult;
                //InitScores();
            }
        }

        StartCoroutine(AnnounceWinner());

    }

    private IEnumerator DissolveAward(Image award)
    {
        //award.transform.position = new Vector3(award.transform.position.x, Mathf.Lerp(award.transform.position.y, award.transform.position.y - 5f, 3f), award.transform.position.z);
        //award.color.a = Mathf.Lerp
        Vector3 startPos = award.transform.position;
        awardDissolve = false;
        float time = 0f;

        Color clr = award.color;
        float yPos = award.transform.position.y;
        Vector3 pos = award.transform.position;
        
        while(time < 1f)
        {
            yield return new WaitForEndOfFrame();
            float alpha = Mathf.Lerp(1, 0, time);
            yPos = Mathf.Lerp(yPos, yPos - 0.5f, time);
            pos.y = yPos;

            clr = award.color;
            clr.a = alpha;

            award.color = clr;
            award.transform.position = pos;

            time += Time.deltaTime / dissolveDuration;
        }

        awardDissolve = true;

        award.transform.position = startPos;
        award.color = Color.clear;
        yield return null;
    }

    private IEnumerator SpawnFries(int playerNum, int awardScore)
    {
        for (int i = 0; i < awardScore; i++)
        {
            float rad = Random.Range(0, 2 * Mathf.PI);

            float x = Mathf.Cos(rad) * 10f;
            float z = Mathf.Sin(rad) * 10f;

            Vector3 spawnPos = new Vector3(x, 0, z) + frySpawner[playerNum].position;

            GameObject fry = Instantiate(awardFry, spawnPos, Quaternion.Euler(0, 0, 0));
            fry.name = "AwardFry";

            fry.GetComponent<AwardFry>().InitializeValues(playerNum, scoreMult);

            yield return new WaitForSeconds((0.75f / awardScore));
        }
    }

    public void IncrementScore(int playerNum, int numToAdd)
    {
        scores[playerNum] += numToAdd;

        //scoreTxts[playerNum].text = scores[playerNum].ToString();
    }

    public void PlayEatSound()
    {
        eatingSource.Play();
    }

    IEnumerator WaitToShowButtons()
    {
        while (AudioManager.Instance.IsPlaying())
        {
            yield return null;
        }

        ShowButtons();
    }

    IEnumerator ResizeGull(float scale, int gull)
    {
        float baseScale = 75f;

        float scaleDif = scale - 75f;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * (1f / 1.7f);

            if (time > 1f)
            {
                time = 1f;
            }

            float changeScale = baseScale + time * scaleDif;

            seagulls[gull].transform.localScale = new Vector3(changeScale, changeScale, changeScale);

            yield return null;
        }
    }
}
