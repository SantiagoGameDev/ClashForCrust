using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutPowerUp : MonoBehaviour
{
    private static DonutPowerUp instance;
    [SerializeField] GameObject d1;
    [SerializeField] GameObject d2;
    public int speed = 1;
    public int sender;
    private bool dienow = false;

    public static DonutPowerUp Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DonutPowerUp>();
            }
            return instance;
        }
    }

    public void RollDonut()
    {
    }

    private void OnTriggerEnter(Collider col)
    {
       // Debug.LogWarning("wtf mate");
        GameObject other = col.gameObject;
        if (other.CompareTag("Obstical"))
        {
            StartCoroutine(Die());
        }
    }

    private void Update()
    {
        if (!dienow)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private IEnumerator Die()
    {
        dienow = true;
        d1.SetActive(false);
        d2.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}