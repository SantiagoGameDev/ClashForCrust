using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenCrust : MonoBehaviour
{
    private static GoldenCrust instance;
    public static GoldenCrust Instance {  get { return instance; } }

    [SerializeField] GameObject circle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        circle.SetActive(true);
        while (true)
        {
            float scale = 1.2f;
            while (scale > 1f)
            {
                scale -= Time.deltaTime * 0.15f;

                if (scale < 1f)
                {
                    scale = 1f;
                }

                float colorScale = 6f - scale * 5f;

                circle.transform.localScale = new Vector3((0.3f * colorScale) + 0.1f, (0.3f * colorScale) + 0.1f);
                Color pccColor = circle.GetComponent<SpriteRenderer>().color;
                circle.GetComponent<SpriteRenderer>().color = new Color(pccColor.r, pccColor.g, pccColor.b, 1f - colorScale);

                yield return null;
            }
        }
    }
}
