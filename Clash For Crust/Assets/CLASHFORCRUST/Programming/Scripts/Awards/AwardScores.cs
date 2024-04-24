using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class AwardScores : MonoBehaviour
{
    [SerializeField] private WorldData.statsType awardStat;
    [SerializeField] private Image awardImage;
    [SerializeField] private List<TMP_Text> scoreText;
    [SerializeField] private List<GameObject> scoreBoxes;

    public Image highlight;

    public Award award;

    public Image selectedPanel;

    [SerializeField] private GameObject SchmoovinBoxes;
    [SerializeField] private GameObject selectedSchmoovin, selectedSchmoovless;

    //public bool wasSelected;

    [SerializeField] private Color gold;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < scoreBoxes.Count; i++)
        {
            if (i > PlayerInputInformation.Instance.GetTotalPlayers() - 1)
            {
                scoreBoxes[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < PlayerInputInformation.Instance.GetTotalPlayers(); i++)
        {
            scoreText[i].text = ((int)PlayerInputInformation.Instance.playerStats[i][awardStat]).ToString();
        }

        foreach (Award awardClass in AwardShowManager.Instance.awardList.awards)
        {
            if (awardClass.statsType == awardStat)
            {
                awardImage.sprite = awardClass.awardImg;
                award = awardClass;

                if (awardClass.name == "Schmoovless")
                {
                    awardImage.enabled = false;
                    SchmoovinBoxes.SetActive(true);
                }

            }
        }

        foreach (Award awardClass in AwardShowManager.Instance.awardList.selectedAwards)
        {
            if (awardClass.statsType == awardStat)
            {
                selectedPanel.enabled = true;
                if (awardClass.name == "Schmoovless")
                {
                    selectedSchmoovless.SetActive(true);
                    selectedPanel.enabled = false;
                }
                if(awardClass.name == "Schmoovin")
                {
                    selectedSchmoovin.SetActive(true);
                    selectedPanel.enabled = false;
                }
                //selectedPanel.sprite = awardClass.awardImg;
            }
        }

        List<int> winners = AwardShowManager.Instance.awardList.CompareStats(award);

        foreach (int winner in winners) 
        {
            if (award.name == "Schmoovless")
            {
                scoreText[winner].color = new Color(123f / 255f, 0f, 0f);
            }
            else
            {
                scoreText[winner].color = gold;
            }
            scoreText[winner].gameObject.transform.localScale *= 1.2f;
        }

        if (award.name == "Schmoovless")
        {
            List<int> winners2 = AwardShowManager.Instance.awardList.CompareStats(AwardShowManager.Instance.awardList.awards[13]);

            foreach (int winner in winners2)
            {
                scoreText[winner].color = gold;
                scoreText[winner].gameObject.transform.localScale *= 1.2f;
            }
        }
    }
}
