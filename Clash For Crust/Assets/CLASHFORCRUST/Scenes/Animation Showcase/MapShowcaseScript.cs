using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapShowcaseScript : MonoBehaviour
{
    [SerializeField] Button btnNext;
    [SerializeField] Button btnPrevious;
    [SerializeField] Button btnChangeView;
    [SerializeField] Button btnRotleft;
    [SerializeField] Button btnRotright;

    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;

   // [SerializeField] Animator animator;

    [SerializeField] TextMeshProUGUI camText;
    [SerializeField] TextMeshProUGUI animText;

    [SerializeField] GameObject[] map = new GameObject[5] ;

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

        if (righthold )
        {
            map[animValue].transform.Rotate(new Vector3(0, -2, 0));
        }
        if(lefthold)
        {
            map[animValue].transform.Rotate(new Vector3(0, 2, 0));
        }
       
      
    }

    private void OnNext () {
        animValue++;
        if (animValue > 4) {
        animValue = 0;
        }
        switch (animValue)
        {
            case 0:
                animText.text = "Map 1";
                map[0].SetActive(true);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 1:
                animText.text = "Map 2";
                map[0].SetActive(false);
                map[1].SetActive(true);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 2:
                animText.text = "Map 3";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(true);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 3:
                animText.text = "Map 4";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(true);
                map[4].SetActive(false);
                break;
            case 4:
                animText.text = "Map 5";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(true);
                break;
        }

    }
    
    private void OnPrev() {
        animValue--;
        if (animValue < 0)
        {
            animValue = 4;
        }
        switch (animValue)
        {
            case 0:
                animText.text = "Map 1";
                map[0].SetActive(true);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 1:
                animText.text = "Map 2";
                map[0].SetActive(false);
                map[1].SetActive(true);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 2:
                animText.text = "Map 3";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(true);
                map[3].SetActive(false);
                map[4].SetActive(false);
                break;
            case 3:
                animText.text = "Map 4";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(true);
                map[4].SetActive(false);
                break;
            case 4:
                animText.text = "Map 5";
                map[0].SetActive(false);
                map[1].SetActive(false);
                map[2].SetActive(false);
                map[3].SetActive(false);
                map[4].SetActive(true);
                break;
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
