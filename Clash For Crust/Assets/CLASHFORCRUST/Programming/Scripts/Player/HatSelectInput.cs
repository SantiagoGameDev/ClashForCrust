using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Data.SqlTypes;

public class HatSelectInput : MonoBehaviour
{
    public int playerIndex;

    GameObject gull;

    private bool ready = false;

    private bool up = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput pi = GetComponent<PlayerInput>();

        playerIndex = pi.playerIndex;

        StartCoroutine(ReadyUp());
    }

    // Update is called once per frame
    void Update()
    {
        if (gull == null)
        {
            gull = GameObject.Find("HatMenuSeagull" + playerIndex);
        }
    }

    public void SubmitButton(InputAction.CallbackContext ctx)
    {
        if (!SceneControl.Instance.loading)
        {

            if (ctx.performed)
            {
                if (ready)
                {
                    if (gull != null)
                    {
                        if (!up)
                        {
                            HatSelection gullScript = gull.GetComponent<HatSelection>();

                            gullScript.LockInHat();
                            up = true;
                        }
                    }
                }
            }
        }
    }

    public void StartButton(InputAction.CallbackContext ctx)
    {
        if(!SceneControl.Instance.loading)
        {
            if (ctx.performed)
                HatSelectionManager.Instance.OnStartPress();
        }
    }

    public void CancelButton(InputAction.CallbackContext ctx)
    {
        if (!SceneControl.Instance.loading)
        {
            if (ctx.performed)
            {
                HatSelectionManager.Instance.HatSelections[playerIndex].OnCancelPress();
                up = false;
                HatSelectionManager.Instance.CheckForReady();
            }
                
        }
    }

    IEnumerator ReadyUp()
    {
        yield return new WaitForSeconds(0.4f);

        ready = true;
    }
}
