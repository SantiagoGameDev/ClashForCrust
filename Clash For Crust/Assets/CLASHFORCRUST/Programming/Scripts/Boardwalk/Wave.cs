using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private float tideTime;
    [SerializeField] private float matchStartGrace;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip waveMovement;

    private Vector3 lowTidePos;
    private Vector3 highTidePos;

    private bool TideSound = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = waveMovement;

        lowTidePos = transform.position;
        highTidePos = new Vector3(transform.position.x, transform.position.y, 35f);
        StartTide();
    }

    public void StartTide()
    {
        StartCoroutine(Tide());
    }

    private IEnumerator Tide()
    {
        yield return new WaitForSeconds(matchStartGrace);


        while (WorldData.Instance.gameOver == false)
        {
            transform.position = lowTidePos;

            yield return new WaitForSeconds(tideTime); // adjust as needed

            if (!TideSound)
            {
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.Hightide, true);
                TideSound = true;
            }

            yield return new WaitForSeconds(4f);

            audioSource.Play();

            while (transform.position.z >= highTidePos.z + 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, highTidePos, Time.deltaTime * 0.5f);
                yield return null;
            }

            yield return new WaitForSeconds(tideTime); // adjust as needed

            WavePowerupSpawner.Instance.SpawnPowerups();

            

            while (transform.position.z <= lowTidePos.z - 0.5f) //Tide back
            {
                if (TideSound)
                {
                    AudioManager.Instance.PlayAudio(AudioManager.AudioType.LowTide, true); //Switch to bye bye tide
                    TideSound = false;
                    yield return new WaitForSeconds(4f);
                    audioSource.Play();
                }
                transform.position = Vector3.Lerp(transform.position, lowTidePos, Time.deltaTime * 0.5f);
                yield return null;
            }
            audioSource.Play();

        }
    }
}