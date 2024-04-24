using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;

public class RulesControl : MonoBehaviour
{
    bool canSubmit = true, canCancel = true;

    private MultiplayerEventSystem mes;
    private void Start()
    {
        mes = GetComponent<MultiplayerEventSystem>();

        mes.playerRoot = GameObject.Find("ChooseMapCanvas");

        mes.firstSelectedGameObject = GameObject.Find("MapPickSlider");
    }

    public void SubmitButtonPress(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!SceneControl.Instance.loading)
            {
                if (!AlterRulesMenu.Instance.onRulesMenu)
                {
                    if (AlterRulesMenu.Instance.sliderSelected)
                    {
                        EventSystem.current.sendNavigationEvents = false;
                        AlterRulesMenu.Instance.PickedMap();
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    AlterRulesMenu.Instance.UpdateToggles();
                }
            }
        }
    }

    public void RulesButtonPress(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!SceneControl.Instance.loading)
            {
                if (!AlterRulesMenu.Instance.onRulesMenu)
                {
                    if (AlterRulesMenu.Instance.sliderSelected)
                    {
                        EventSystem.current.sendNavigationEvents = false;
                        AlterRulesMenu.Instance.OpenRules();
                    }
                }
            }
        }
    }

    public void BackButtonPress(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!SceneControl.Instance.loading)
            {
                if (AlterRulesMenu.Instance.onRulesMenu)
                {
                    if (EventSystem.current.sendNavigationEvents)
                    {
                        AlterRulesMenu.Instance.CloseRules();
                    }
                }
                else
                {
                    AlterRulesMenu.Instance.GoBack();
                }
            }
        }
    }

    public void ScrewButtonPress(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!SceneControl.Instance.loading)
            {
                if (AlterRulesMenu.Instance.onRulesMenu)
                {
                    if (EventSystem.current.sendNavigationEvents)
                    {
                        AlterRulesMenu.Instance.UnlockScrewMode();
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (mes)
        {
            if (AlterRulesMenu.Instance.onRulesMenu)
            {
                if (mes.playerRoot.name == "ChooseMapCanvas")
                {
                    mes.playerRoot = GameObject.Find("RuleAlterCanvas");
                    string sliderName = "";
                    switch (AlterRulesMenu.Instance.selectedSlider)
                    {
                        case 0:
                            sliderName = "GameTimeSlider";
                            break;
                        case 1:
                            sliderName = "HealthSlider";
                            break;
                        case 2:
                            sliderName = "CrustSlider";
                            break;
                        case 3:
                            sliderName = "PowerupSlider";
                            break;
                        case 4:
                            sliderName = "FrySlider";
                            break;
                        case 5:
                            sliderName = "AwardSlider";
                            break;
                        case 6:
                            sliderName = "ScrewSlider";
                            break;
                    }
                    mes.firstSelectedGameObject = GameObject.Find(sliderName);
                    mes.firstSelectedGameObject.GetComponent<Slider>().Select();
                }
            }
            else
            {
                if (mes.playerRoot.name != "ChooseMapCanvas")
                {
                    mes.playerRoot = GameObject.Find("ChooseMapCanvas");
                    mes.firstSelectedGameObject = GameObject.Find("MapPickSlider");
                    mes.firstSelectedGameObject.GetComponent<Slider>().Select();
                }
            }
        }
    }

    IEnumerator ButtonCoolDown(bool button)
    {
        if (!button)
        {
            canSubmit = false;
        }
        else
        {
            canCancel = false;
        }
        yield return new WaitForSeconds(0.15f);
        if (!button)
        {
            canSubmit = true;
        }
        else
        {
            canCancel = true;
        }
    }
}
