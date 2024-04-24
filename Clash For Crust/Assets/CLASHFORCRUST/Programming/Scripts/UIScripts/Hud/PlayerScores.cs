using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScores : MonoBehaviour
{
    [SerializeField] public int playerIndex;
    [SerializeField] private Image yellowCircle;
    private TMP_Text scoreText;

    private bool updatingCC;
    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.enabled = false;

        updatingCC = false;

        FeatherEvents.Instance.OnPlayerGainedCalories += UpdateScoreCount;

        yellowCircle.transform.position = HudManager.Instance.playerCircles[playerIndex].transform.position;
        yellowCircle.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!RoundManager.Instance.GameStart)
        {
            if (playerIndex < RoundManager.Instance.playersRequiredToStart)
            {
                scoreText.enabled = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        if (RoundManager.Instance.hideHud)
        {
            scoreText.enabled = false;
        }

    }

    public void UpdateScoreCount(int player)
    {
        if (player == playerIndex)
        {
            WorldData.Instance.players[player].TryGetValue(WorldData.statsType.calorieCount, out float calorieCount);

            StartCoroutine(ScoreCountAnim(calorieCount));
        }
        
    }

    IEnumerator ScoreCountAnim(float calorieCount)
    {
        updatingCC = true;

        scoreText.text = calorieCount.ToString();
        updatingCC = false;

        yellowCircle.enabled = true;

        GameObject playerCrustCircle = RoundManager.Instance.players[playerIndex].crustCircle;
        playerCrustCircle.SetActive(true);

        float scale = 1.2f;
        while (!updatingCC && scale > 1f)
        {
            scale -= Time.deltaTime * 0.3f;

            if (scale < 1f)
            {
                scale = 1f;
            }

            transform.localScale = new Vector3(scale, scale, 0f);

            float colorScale = 6f - scale * 5f;
            scoreText.color = new Color(1f, 1f, colorScale);

            if (RoundManager.Instance.HoldingCrust == playerIndex)
            {
                playerCrustCircle.transform.localScale = new Vector3((0.15f * colorScale) + 0.1f, (0.15f * colorScale) + 0.1f);
                Color pccColor = playerCrustCircle.GetComponent<SpriteRenderer>().color;
                playerCrustCircle.GetComponent<SpriteRenderer>().color = new Color(pccColor.r, pccColor.g, pccColor.b, 1f - colorScale);

                yellowCircle.transform.localScale = new Vector3((2f * colorScale) +5f, (2f * colorScale) + 5f, 0f);
                yellowCircle.color = new Color(yellowCircle.color.r, yellowCircle.color.g, yellowCircle.color.b, 1f - colorScale);
            }
            else
            {
                yellowCircle.enabled = false;
                playerCrustCircle.SetActive(false);
            }
            yield return null;
        }

        yellowCircle.enabled = false;
        playerCrustCircle.SetActive(false);

    }
}
