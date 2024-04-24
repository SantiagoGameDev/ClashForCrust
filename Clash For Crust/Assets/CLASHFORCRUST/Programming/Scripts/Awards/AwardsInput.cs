using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class AwardsInput : MonoBehaviour
{
    public void ShowStats(InputAction.CallbackContext ctx)
    {
        AwardScoreMenu.Instance.YPressed(ctx);
    }

    public void GoBack(InputAction.CallbackContext ctx)
    {
        AwardScoreMenu.Instance.BPressed(ctx);
    }
}
