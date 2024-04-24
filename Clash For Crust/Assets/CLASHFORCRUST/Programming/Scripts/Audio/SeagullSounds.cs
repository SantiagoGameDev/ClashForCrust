using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullSounds : MonoBehaviour
{
    public enum SeagullSoundType
    {
        Idle, Squawk, Spin, Smash, Shot, Stun, Hurt, Donut, Chili, PopCorn, Firework, Pickup, CrustGurgle, Screw, ShotSplat, FireworkExplode
    }

    [SerializeField] AudioSource seagullSource;
    [SerializeField] AudioSource eatingSource;
    [SerializeField] AudioClip idle, squawk, spin, smash, shot, stun, hurt, donut, chili, popcorn, firework, pickup, crustGurgle, screw, shotSplat, fireworkExplode;

    public void PlaySound(SeagullSoundType type)
    {
        switch (type)
        {
            case SeagullSoundType.Idle:
                seagullSource.clip = idle;
                break;
            case SeagullSoundType.Squawk:
                seagullSource.clip = squawk;
                break;
            case SeagullSoundType.Spin:
                seagullSource.clip = spin;
                break;
            case SeagullSoundType.Smash:
                seagullSource.clip = smash;
                break;
            case SeagullSoundType.Shot:
                seagullSource.clip = shot;
                break;
            case SeagullSoundType.Stun:
                seagullSource.clip = stun;
                break;
            case SeagullSoundType.Hurt:
                seagullSource.clip = hurt;
                break;
            case SeagullSoundType.Donut:
                seagullSource.clip = donut;
                break;
            case SeagullSoundType.Chili:
                seagullSource.clip = chili;
                break;
            case SeagullSoundType.PopCorn:
                seagullSource.clip = popcorn;
                break;
            case SeagullSoundType.Firework:
                seagullSource.clip = firework;
                break;
            case SeagullSoundType.CrustGurgle:
                seagullSource.clip = crustGurgle;
                break;
            case SeagullSoundType.Screw:
                seagullSource.clip = screw;
                break;
            case SeagullSoundType.ShotSplat:
                seagullSource.clip = shotSplat;
                break;
            case SeagullSoundType.FireworkExplode:
                seagullSource.clip = fireworkExplode;
                break;
            default:
                break;
        }

        seagullSource.Play();
    }

    public void PlayEating()
    {
        eatingSource.Play();
    }

}
