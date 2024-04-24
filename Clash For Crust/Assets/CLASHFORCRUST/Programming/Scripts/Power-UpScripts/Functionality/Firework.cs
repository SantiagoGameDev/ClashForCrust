using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static PlayerController;

public class Firework : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject fWObject;
    [SerializeField] private GameObject dynamiteObj;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fwExplosionGO;
    [SerializeField] private GameObject fwHitbox;

    private bool collided = false;
    private Vector3 direction;
    private bool fireworkActive;
    private SeagullSounds seagullSounds;

    private void Start()
    {
        seagullSounds = this.GetComponent<SeagullSounds>();

        if (AudioManager.Instance.pirateMode)
            fWObject = dynamiteObj;
    }

    public void FireworkAttack()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        //playerController = gameObject.GetComponent<PlayerController>();
        direction = transform.forward;
        collided = false;
        fireworkActive = true;
        fwHitbox.SetActive(true);
        //fWObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (rb != null && collided == false && fireworkActive)
        {
            rb.AddForce(direction * 70000f * Time.fixedDeltaTime);
            playerController.controllable = false;
        }
    }

    private IEnumerator EndLag()
    {
        fwExplosionGO.SetActive(true);
        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.FireworkExplode);
        fwHitbox.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        fwExplosionGO.SetActive(false);
        if (playerController.activePowerup != ActivePowerup.CRUST) // make firework check if true gthen srt activ e fslade
        {
            playerController.activePowerup = ActivePowerup.NONE;
        }
        playerController.controllable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning(other.tag + "   " + fireworkActive);
        if ((other.CompareTag("Obstical") || other.CompareTag("PlayerCollider")) && fireworkActive)
        {
            //Debug.LogWarning("Firework scene from any anime");
            rb.velocity = Vector3.zero;
            collided = true;
            fireworkActive = false;
            fWObject.SetActive(false);
            animator.SetTrigger("FireworkEnd");
            StartCoroutine(EndLag());
        }
    }
}