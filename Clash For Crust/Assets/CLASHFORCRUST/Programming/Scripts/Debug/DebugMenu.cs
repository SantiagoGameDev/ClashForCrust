using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> PowerUpGOs = new List<GameObject>();

    [SerializeField] GameObject DebugUI;

    private bool debugOn = false;

    private MapController mapController;

    private void Awake()
    {
        mapController = FindObjectOfType<MapController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //Debug.Log("Debug menu pressed");
            ToggleDebug(debugOn);
        }
    }

    public void ToggleDebug(bool debugState)
    {
        DebugUI.SetActive(!debugOn);
        debugOn = !debugOn;
    }

    public void FillShotBar(int player)
    {
        //Make sure its at 0 before incrementing it
        RoundManager.Instance.players[player].ResetShotMeter();

        //
        for (int i = 0; i < 5; i++)
        {
            RoundManager.Instance.players[player].IncrementShotMeter();
        }
        
    }

    public void StunPlayer(int player)
    {
        RoundManager.Instance.players[player].SetStamina(1);
        RoundManager.Instance.players[player].TakeDamage(1);
    }

    public void DropCrust()
    {
        if(RoundManager.Instance.HoldingCrust != -1)
            RoundManager.Instance.players[RoundManager.Instance.HoldingCrust].RevertBack();
    }

    public void SelectMapLayout(int layout)
    {
        mapController.SelectLayout(layout);
    }

    public void EndMatch()
    {
        RoundManager.Instance.GameTime = 1f;
    }

    public void GivePopCorn(int player)
    {
        Instantiate(PowerUpGOs[0], RoundManager.Instance.players[player].transform.position, Quaternion.identity);
    }
    public void GiveChili(int player)
    {
        Instantiate(PowerUpGOs[1], RoundManager.Instance.players[player].transform.position, Quaternion.identity);
    }
    public void GiveDonut(int player)
    {
        Instantiate(PowerUpGOs[2], RoundManager.Instance.players[player].transform.position, Quaternion.identity);
    }
    public void GiveFirework(int player)
    {
        Instantiate(PowerUpGOs[3], RoundManager.Instance.players[player].transform.position, Quaternion.identity);
    }
    public void GiveFry(int player)
    {
        Instantiate(PowerUpGOs[4], RoundManager.Instance.players[player].transform.position, Quaternion.identity);
    }
}
