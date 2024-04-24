using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherEvents : MonoBehaviour
{
    private static FeatherEvents instance;
    public static FeatherEvents Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public delegate void PlayerLostHealth(int player);
    public event PlayerLostHealth OnPlayerLostHealth;

    public delegate void PlayerGainedHealth(int player);
    public event PlayerGainedHealth OnPlayerGainedHealth;

    public delegate void PlayerGainedCalories(int player);
    public event PlayerGainedCalories OnPlayerGainedCalories;

    private float timer = 0f;
    public float Timer { get { return timer; } }

    public void ThisPlayerLostHealth(int player)
    {
        OnPlayerLostHealth(player);
    }

    public void ThisPlayerGainedHealth(int player)
    {
        OnPlayerGainedHealth(player);
    }

    public void ThisPlayerGainedCalories(int player)
    {
        OnPlayerGainedCalories(player);
    }
    //timer that the feathers use to know what part of the animation theyre on
    private void Update()
    {
        timer += Time.deltaTime * 1.5f;

        if (timer > Mathf.PI * 2)
        {
            timer = 0f;
        }
    }
}
