using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleChoiceInfo : MonoBehaviour
{
    private static RuleChoiceInfo instance;
    public static RuleChoiceInfo Instance { get { return instance; } }

    public float gameTime;
    public int maxHealth;
    public bool crustOn;
    public int crustValue;
    public bool powerupsOn;
    public float powerupSpawnRate;
    public bool fryOn;
    public float frySpawnRate;
    public bool awardsOn;
    public int numAwards;
    public bool screwMode;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
    }
}
