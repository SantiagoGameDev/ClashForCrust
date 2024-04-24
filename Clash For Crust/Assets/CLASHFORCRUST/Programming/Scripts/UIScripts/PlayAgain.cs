using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAgain : MonoBehaviour
{
    public void PlayAgainBtn()
    {
        switch (SceneControl.Instance.GetPrevScene())
        {
            case "Carnival":
                SceneControl.Instance.LoadAScene(2, "Carnival");
                break;
            case "Boardwalk Reese":
                SceneControl.Instance.LoadAScene(3, "Boardwalk Reese");
                break;
            case "PiratePeak":
                SceneControl.Instance.LoadAScene(4, "PiratePeak");
                break;
            default:
                break;
        }
    }

    public void MainMenuBtn()
    {
        SceneControl.Instance.LoadAScene(0, "Menu");
    }

    public void ChangeMapBtn()
    {
        SceneControl.Instance.LoadSceneFromName("RulesMenu");
    }
}
