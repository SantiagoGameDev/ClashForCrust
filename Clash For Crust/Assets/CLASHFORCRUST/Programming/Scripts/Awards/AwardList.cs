//JR AM
//02/14/24
//Awards List

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Award class
[System.Serializable]
public class Award
{
    public string name;
    public int calorieCountBonus;
    public bool low = false; //If true it will search for the player with the lowest score instead
    public WorldData.statsType statsType;
    public AudioManager.AudioType audioType;
    public Sprite awardImg;
}

public class AwardList : MonoBehaviour
{
    [SerializeField] int awardCount; //How many awards will be chosen. This can be changed in the rules menu

    [SerializeField] public List<Award> awards;

    private List<Award> awardsCopy = new List<Award>();
    public List<Award> selectedAwards;

    private void Awake()
    {
        //Debug.Log("Award List Awake");

        selectedAwards = new List<Award>();

        foreach (Award award in awards)
        {
            awardsCopy.Add(award);
        }

        if (RuleChoiceInfo.Instance.awardsOn)
            SelectAwards(RuleChoiceInfo.Instance.numAwards);

    }
    private void Start()
    {
    }

    //Run when the award screen starts. Selects however many awards to be used
    public void SelectAwards(int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            int rngNum = Random.Range(0, awardsCopy.Count); //Generate random number

            selectedAwards.Add(awardsCopy[rngNum]); //Add selected num to awards

            awardsCopy.RemoveAt(rngNum); //Remove it from the copy list
        }
    }

    public void StartAwardShow()
    {
        List<int> awardWinner = new List<int>();

        foreach (Award award in selectedAwards)
        {
            awardWinner = CompareStats(award); //Find the person who had the highest stat
            GrantAward(award, awardWinner);

            //TODO: Do visual stuff here
        }
    }

    //Returns which number player has the stat for the award
    public List<int> CompareStats(Award award)
    {
        float highestStat = 0;
        List<int> bestPlayer = new List<int>();

        if (!award.low)
        {
            for (int i = 0; i < PlayerInputInformation.Instance.GetTotalPlayers(); i++)
            {
                if (PlayerInputInformation.Instance.playerStats[i][award.statsType] > highestStat)
                {
                    bestPlayer = new List<int>();
                    bestPlayer.Add(i);
                    highestStat = PlayerInputInformation.Instance.playerStats[i][award.statsType];
                }
                else if (PlayerInputInformation.Instance.playerStats[i][award.statsType] == highestStat)
                {
                    bestPlayer.Add(i);
                }
            }
        }
        else if (award.low)
        {
            highestStat = Mathf.Infinity;
            for (int i = 0; i < PlayerInputInformation.Instance.GetTotalPlayers(); i++)
            {
                if (PlayerInputInformation.Instance.playerStats[i][award.statsType] < highestStat)
                {
                    bestPlayer = new List<int>();
                    bestPlayer.Add(i);
                    highestStat = PlayerInputInformation.Instance.playerStats[i][award.statsType];
                }
                else if (PlayerInputInformation.Instance.playerStats[i][award.statsType] == highestStat)
                {
                    bestPlayer.Add(i);
                }
            }
        }

        //Debug.Log("Best player for " + award.statsType.ToString() + " is " + bestPlayer);

        return bestPlayer;
    }

    public void GrantAward(Award award, List<int> playerIds)
    {
        //Debug.Log("Award: " + award.name + " to player " + playerId);

        foreach (int player in playerIds)
        {
            PlayerInputInformation.Instance.playerStats[player][WorldData.statsType.calorieCount] += award.calorieCountBonus; //Give them the calorie bonus for that award
        }
    }

    private IEnumerator AwardShowDelay()
    {
        yield return new WaitForSeconds(2f);
    }
}
