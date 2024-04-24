using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HatPIM : MonoBehaviour
{
    PlayerInputManager pim;
    // Start is called before the first frame update
    void Awake()
    {
        pim = GetComponent<PlayerInputManager>();

        pim.DisableJoining();

    }

    IEnumerator EnableJoining()
    {
        yield return new WaitForSeconds(1f);

        pim.EnableJoining();
    }

    public void StartJoining()
    {
        StartCoroutine(EnableJoining());
    }
}
