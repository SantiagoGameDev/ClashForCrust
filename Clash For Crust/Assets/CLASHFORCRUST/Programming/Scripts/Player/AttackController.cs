using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private PlayerController playerController;

    //Cooldowns
    [SerializeField] private float spinCooldown;
    [SerializeField] private float smashCooldown;

    //bools
    private bool canSpin;
    private bool canSmash;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        canSpin = true;
        canSmash = true;
    }

    private void ActivateSpin()
    {
        //Call Seagull Spin from Player
    }

    private void ActivateSmash()
    {
        //Call Seagull Smash from Player
    }
}
