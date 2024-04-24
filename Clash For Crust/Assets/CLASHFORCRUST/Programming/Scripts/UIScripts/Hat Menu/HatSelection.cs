using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HatSelection : MonoBehaviour
{
    [SerializeField] private SelectionWheel hatWheel;
    [SerializeField] private SelectionWheel skinWheel;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lockinSound;

    private int hatNum;
    private int skinNum;

    private Transform hatSpawner;
    private List<GameObject> hatList;

    public int selectedHat;
    public int selectedSkin;

    private SkinnedMeshRenderer rend;

    [SerializeField] private bool hatSelected;

    public Slider hatSlider;
    public Slider skinSlider;

    private Animator animator;
    private int playerID;
    public int PlayerID
    { get { return playerID; } }

    [SerializeField] private GameObject lockInObject;
    [SerializeField] private GameObject pressAToJoinObj;

    private int prevHatSliderVal;
    private int prevSkinSliderVal;

    private bool hatSliderBool, skinSliderBool;

    private void Awake()
    {
        playerID = int.Parse(gameObject.name.Substring(name.LastIndexOf("l") + 1, 1));

        hatSpawner = GetComponentInChildren<HatSpawner>().transform;

        hatList = new List<GameObject>();
        foreach (GameObject hat in HatsAndSkins.Instance.Hats)
        {
            GameObject hatModel = Instantiate(hat, hatSpawner.transform);
            hatList.Add(hatModel);
            hatModel.SetActive(false);
        }

        Slider[] canvasSliders = GameObject.Find("CanvasP" + (playerID + 1)).gameObject.GetComponentsInChildren<Slider>();
        foreach (Slider slider in canvasSliders)
        {
            string hatSliderName = "P" + (playerID + 1) + "HatSlider";
            //Debug.Log("hatSliderName " + hatSliderName);
            if (slider.name == "P" + (playerID + 1) + "HatSlider")
            {
                hatSlider = slider;
            }

            if (slider.name == "P" + (playerID + 1) + "SkinSlider")
            {
                skinSlider = slider;
            }
        }

        hatSelected = false;
        hatNum = 0;
        skinNum = 0;
        selectedHat = 0;
        hatSliderBool = true;
        skinSliderBool = true;

        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        prevHatSliderVal = 1;
        prevSkinSliderVal = 1;

        if (hatSlider != null)
        {
            hatSlider.maxValue = hatList.Count + 1;
            hatSlider.value = 1;
        }

        hatSlider.onValueChanged.AddListener(delegate { SliderChange(); });
        skinSlider.onValueChanged.AddListener(delegate { SkinSliderChange(); });
    }

    private void Update()
    {
        if (hatWheel.CanSpin())
        {
            hatSliderBool = true;
            if (hatSlider.interactable == false)
            {
                hatSlider.interactable = true;

                HatSelectInput[] hsis = FindObjectsOfType<HatSelectInput>();
                HatSelectInput hsi = new HatSelectInput();
                foreach (HatSelectInput hsi1 in hsis)
                {
                    if (hsi1.playerIndex == playerID)
                    {
                        hsi = hsi1;
                    }
                }
                hsi.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(hatSlider.gameObject);
            }
        }
        if (!hatWheel.CanSpin() || hatSelected)
            hatSlider.interactable = false;

        if (skinWheel.CanSpin())
        {
            skinSliderBool = true;
            if (skinSlider.interactable == false)
            {
                skinSlider.interactable = true;

                HatSelectInput[] hsis = FindObjectsOfType<HatSelectInput>();
                HatSelectInput hsi = new HatSelectInput();
                foreach (HatSelectInput hsi1 in hsis)
                {
                    if (hsi1.playerIndex == playerID)
                    {
                        hsi = hsi1;
                        hsi.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(skinSlider.gameObject);
                    }
                }
            }
        }
        if (!skinWheel.CanSpin() || hatSelected)
            skinSlider.interactable = false;
    }

    public void SelectRight()
    {
        if (!hatSelected)
        {
            hatNum++;

            if (hatNum == hatList.Count)
                hatNum = 0;

            //Debug.Log("Hatnum: " + hatNum);

            HatChange(hatNum);
        }
    }

    public void SelectLeft()
    {
        if (!hatSelected)
        {
            hatNum--;

            if (hatNum == -1)
                hatNum = 0;

            //Debug.Log("Hatnum: " + hatNum);

            HatChange(hatNum);
        }
    }

    public void LockInHat()
    {
        //Debug.Log("Hat locked in");
        hatSelected = true;
        animator.SetTrigger("Selected");
        lockInObject.SetActive(true);
        audioSource.Play();

        PlayerInputInformation.Instance.UpdateHat(playerID, selectedHat);
        PlayerInputInformation.Instance.UpdateSkin(playerID, selectedSkin);

        HatSelectionManager.Instance.CheckForReady();
    }

    public bool LockedIn()
    {
        return hatSelected;
    }

    public void HatChange(int newHat)
    {
        hatList[selectedHat].gameObject.SetActive(false);
        hatList[newHat].gameObject.SetActive(true);
        selectedHat = newHat;

        //Debug.Log("hat selected " + selectedHat);
    }

    public void SkinChange(int skin)
    {
        rend.material = HatsAndSkins.Instance.Skins[skin];
        selectedSkin = skin;

        //Debug.Log("skin selected " + selectedSkin);
    }

    public void SliderChange()
    {
        if (hatSliderBool)
        {
            if (hatWheel.CanSpin()) //if the wheel is available to spin let  the user spin it
            {
                hatSliderBool = false;

                bool isRight = false;

                if (prevHatSliderVal < hatSlider.value)
                {
                    isRight = true;

                    if (prevHatSliderVal == 0 && hatSlider.value != hatSlider.maxValue)
                    {
                        isRight = false;
                    }
                }
                else if (prevHatSliderVal > hatSlider.value)
                {
                    isRight = false;

                    if (hatSlider.value == 2 && prevHatSliderVal == hatSlider.maxValue)
                    {
                        isRight = true;
                    }
                }

                prevHatSliderVal = (int)hatSlider.value;

                if (hatSlider.value > hatSlider.maxValue - 1)
                {
                    hatSlider.value = 1;
                }

                if (hatSlider.value < 1)
                {
                    hatSlider.value = HatsAndSkins.Instance.Hats.Count;
                }

                int hatChoice = (int)hatSlider.value - 1;

                if (isRight)
                {
                    hatWheel.SpinRight();
                }
                else
                {
                    hatWheel.SpinLeft();
                }

                HatChange(hatChoice);
            }
        }
    }

    public void SkinSliderChange()
    {
        if (skinSliderBool)
        {
            if (skinWheel.CanSpin()) //if the wheel is available to spin let  the user spin it
            {
                skinSliderBool = false;

                bool isRight = false;

                if (prevSkinSliderVal < skinSlider.value)
                {
                    isRight = true;

                    if (prevSkinSliderVal == 0 && skinSlider.value != skinSlider.maxValue)
                    {
                        isRight = false;
                    }
                }
                else if (prevSkinSliderVal > skinSlider.value)
                {
                    isRight = false;

                    if (skinSlider.value == 2 && prevSkinSliderVal == skinSlider.maxValue)
                    {
                        isRight = true;
                    }
                }

                prevSkinSliderVal = (int)skinSlider.value;

                if (skinSlider.value > skinSlider.maxValue - 1)
                {
                    skinSlider.value = 1;
                }

                if (skinSlider.value < 1)
                {
                    skinSlider.value = HatsAndSkins.Instance.Skins.Count;
                }

                int skinChoice = (int)skinSlider.value - 1;

                if (isRight)
                {
                    skinWheel.SpinRight();
                }
                else
                {
                    skinWheel.SpinLeft();
                }

                SkinChange(skinChoice);
            }
        }

    }

    public void PlayedJoined()
    {
        pressAToJoinObj.SetActive(false);
    }

    public void OnCancelPress()
    {
        if (hatSelected)
        {
            //Debug.Log("Unlocking in for player " + playerID);
            hatSelected = false;
            lockInObject.gameObject.SetActive(false);
        }
        else if (!hatSelected)
            SceneControl.Instance.LoadAScene(0, "Menu");
        
    }
}