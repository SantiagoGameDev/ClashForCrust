using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimShowcaseScript : MonoBehaviour
{
    [SerializeField] Button btnNext;
    [SerializeField] Button btnPrevious;
    [SerializeField] Button btnChangeView;
    [SerializeField] Button btnRotleft;
    [SerializeField] Button btnRotright;

    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;

    [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI camText;
    [SerializeField] TextMeshProUGUI animText;

    [SerializeField] GameObject gull;

    private int animValue = 0;
    private bool lefthold = false;
    private bool righthold = false;
    
    void Start()
    {
        btnNext.onClick.AddListener(OnNext);
        btnPrevious.onClick.AddListener(OnPrev);
        btnChangeView.onClick.AddListener(OnChangeView);

        camera1.enabled = true;
        camera2.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetInteger("AnimShow", animValue);

        if (righthold )
        {
            gull.transform.Rotate(new Vector3(0, -2, 0));
        }
        if(lefthold)
        {
            gull.transform.Rotate(new Vector3(0, 2, 0));
        }

        switch (animValue)
        {
            case 0:
                animText.text = "Idle";
                break;
            case 1:
                animText.text = "Walk";
                break;
            case 2:
                animText.text = "Idle - Wing Smell";
                break;
            case 3:
                animText.text = "Idle - Wing Flap";
                break;
            case 4:
                animText.text = "Knockback";
                break;
            case 5:
                animText.text = "Seagull Spin";
                break;
            case 6:
                animText.text = "Seagull Smash/Step";
                break;
            case 7:
                animText.text = "Firebreath";
                break;
            case 8:
                animText.text = "Donut";
                break;
            case 9:
                animText.text = "Crust Pickup";
                break;
            case 10:
                animText.text = "Powerup Pickup";
                break;
            case 11:
                animText.text = "Popcorn Fire";
                break;
            case 12:
                animText.text = "Seagull Shot";
                break;
            case 13:
                animText.text = "Firework";
                break;
            case 14:
                animText.text = "Stunned 1";
                break;
            case 15:
                animText.text = "Stunned 2";
                break;
        }
    }

    private void OnNext () {
        animValue++;
        if (animValue > 15) {
        animValue = 0;
        }
        
    }
    
    private void OnPrev() {
        animValue--;
        if (animValue < 0)
        {
            animValue = 15;
        }
    }

    private void OnChangeView() {
        if (camera1.enabled == true)
        {
            camera1.enabled = false;
            camera2.enabled = true;
            camText.text = "Game View";
        }
        else 
        {
            camera2.enabled = false;
            camera1.enabled = true;
            camText.text = "Close Up";
        }
    }

    public void OnRotLeftPress() {

        // gull.transform.Rotate(new Vector3(0, 1, 0));
        lefthold = true;
    }
    public void OnRotLeftRelease()
    {
        lefthold = false;
       // gull.transform.Rotate(new Vector3(0, 1, 0));
    }

    public void OnRotRightPress() {
        
        righthold = true;

    }
    public void OnRotRightRelease()
    {
        
        righthold= false;
    }
}
