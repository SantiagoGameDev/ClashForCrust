using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChilliBreath : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public int playerNum;
    private bool breathActive;

    public void ChilliBreathAttack()
    {
        if (!breathActive)
        {
            breathActive = true;

            BoxCollider collide = this.GetComponent<BoxCollider>();
            ParticleSystem PS = this.GetComponent<ParticleSystem>();
            playerNum = playerController.playerNum;
            collide.enabled = true;
            PS.Play();
            StartCoroutine(TimedFire(collide, PS));
        }
        
    }

    // Update is called once per frame
   private IEnumerator TimedFire(BoxCollider collide, ParticleSystem PS)
    {
        playerController.canPickUpCrust = false;
        playerController.seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Chili);
        yield return new WaitForSeconds(3);

        if(playerController.activePowerup != PlayerController.ActivePowerup.CRUST)
            playerController.activePowerup = PlayerController.ActivePowerup.NONE;
        playerController.canPickUpCrust = true;
        collide.enabled = false;
        PS.Stop();
        breathActive = false;

    }
}
