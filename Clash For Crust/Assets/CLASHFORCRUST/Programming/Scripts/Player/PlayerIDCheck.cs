using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIDCheck : MonoBehaviour
{
    PlayerInput pi;

    [SerializeField]
    int playerID;

    // Start is called before the first frame update
    void Start()
    {
        pi = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        playerID = pi.playerIndex; 
    }
}
