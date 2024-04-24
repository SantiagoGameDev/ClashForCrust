using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MapSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnSelect(BaseEventData bed)
    {
        AlterRulesMenu.Instance.sliderSelected = true;
        AlterRulesMenu.Instance.AButton.GetComponentInChildren<TMP_Text>().text = "Select Map";
    }

    public void OnDeselect(BaseEventData bed) 
    {
        AlterRulesMenu.Instance.sliderSelected = false;
    }
}
