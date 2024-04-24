using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> layouts = new List<GameObject>();

    GameObject activeLayout;

    public void ChangeLayout()
    {
        int randNum = Random.Range(0, layouts.Count);

        for (int i = 0; i < layouts.Count; i++)
        {
            if (i != randNum)
            {
                layouts[i].SetActive(false);
            }
            else
            {
                layouts[i].SetActive(true);
            }
        }

        RoundManager.Instance.GetSpawners();

    }

    public void SelectLayout(int layoutNum)
    {
        GameObject chosenLayout = new GameObject();

        chosenLayout = layouts[layoutNum];
        //Debug.Log("Chosen layout: " + chosenLayout);

        activeLayout = chosenLayout;

        for (int i = 0; i < layouts.Count; i++)
        {
            if (layouts[i] != activeLayout)
            {
                layouts[i].SetActive(false);
            }
            else
            {
                layouts[i].SetActive(true);
            }
        }

        RoundManager.Instance.GetSpawners();

    }

}
