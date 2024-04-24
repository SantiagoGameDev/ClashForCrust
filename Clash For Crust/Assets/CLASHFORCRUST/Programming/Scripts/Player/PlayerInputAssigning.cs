using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerInputAssigning : MonoBehaviour
{
    [SerializeField] PlayerInputManager pim;

    public void SetPlayerInfo(PlayerInput newPlayer)
    {
        //pim.DisableJoining();
        
        //PlayerInput[] playerInput = FindObjectsOfType<PlayerInput>();

        //PlayerInput newPlayer = playerInput[0];

        

        int playerNum = newPlayer.playerIndex + 1;

        StartCoroutine(WaitForLoading(newPlayer, playerNum));
    }

    private IEnumerator PlayerJoinDelay()
    {
        yield return new WaitForSeconds(0.1f);
        pim.EnableJoining();
    }

    private IEnumerator WaitForLoading(PlayerInput newPlayer, int playerNum)
    {
        yield return new WaitUntil(() => HatSelectionManager.Instance);

        MultiplayerEventSystem mes = newPlayer.gameObject.GetComponent<MultiplayerEventSystem>();

        if (HatSelectionManager.Instance)
        {
            //Debug.LogError("Make the gull visible");
            HatSelectionManager.Instance.MakeGullVisible(newPlayer.playerIndex);
        }


        mes.playerRoot = GameObject.Find("CanvasP" + playerNum);
        mes.firstSelectedGameObject = mes.playerRoot.gameObject.GetComponentInChildren<Slider>().gameObject;
        //Debug.LogError("Currently selected object: " + mes.currentSelectedGameObject);
        mes.SetSelectedGameObject(mes.firstSelectedGameObject);
        //Debug.LogError("Currently selected object again: " + mes.currentSelectedGameObject);
        //Debug.LogError("Currently selected object set active: " + mes.currentSelectedGameObject.gameObject.activeSelf);
        PlayerInputInformation.Instance.GetPlayerInfo(newPlayer);

        StartCoroutine(PlayerJoinDelay());
    }
}
