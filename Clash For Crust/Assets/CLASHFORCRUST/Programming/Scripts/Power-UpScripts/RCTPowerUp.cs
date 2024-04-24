using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCTPowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeLeft;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject screw;
    [SerializeField] private GameObject spawnPos;
    [SerializeField] private AudioClip coasterRide;
    [SerializeField] private AudioSource audioSource;
    private bool playSound = false;
    void Start()
    {
        audioSource.clip = coasterRide;
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundManager.Instance.PowerOn)
            timeLeft -= Time.deltaTime;

        if(timeLeft <= 2f && !playSound)
        {
            audioSource.Play();
            playSound = true;
        }


        if (timeLeft < 0)
        {
            timeLeft = 60;
            anim.SetTrigger("Start");
            playSound = false;
        }
    }

    public void ScrewMe()
    {
        anim.ResetTrigger("Start");
        GameObject SpawnedScrew = Instantiate(screw); 
        SpawnedScrew.transform.position = spawnPos.transform.position;
        SpawnedScrew.GetComponent<Rigidbody>().AddForce(spawnPos.transform.forward * 700);
        playSound = false;
    }
}
