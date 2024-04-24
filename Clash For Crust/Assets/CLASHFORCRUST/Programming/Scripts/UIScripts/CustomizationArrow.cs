using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CustomizationArrow : MonoBehaviour, ISelectHandler
{
    [SerializeField] GameObject hatArrow, skinArrow;

    private enum CustomizationType { Hat, Skin };

    [SerializeField] private CustomizationType hatOrSkin;

    public void OnSelect(BaseEventData bsd)
    {

        switch (hatOrSkin)
        {
            case CustomizationType.Hat:
                skinArrow.SetActive(false);
                hatArrow.SetActive(true);
                break;
            case CustomizationType.Skin:
                skinArrow.SetActive(true);
                hatArrow.SetActive(false);
                break;

        }

    }
}